using System.Data;
using fourplay.Data;
using fourplay.Models;
using fourplay.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using fourplay.Models.Enum;
using fourplay.Helpers;
namespace fourplay.Services;
public class LeaderboardService : ILeaderboardService {
    private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; }
    private readonly IMemoryCache _memory;
    private readonly ISpreadCalculator _spreadCalculator;
    public LeaderboardService(IMemoryCache memoryCache, IDbContextFactory<ApplicationDbContext> dbFactory, ISpreadCalculator spreadCalculator) {
        _memory = memoryCache;
        _dbContextFactory = dbFactory;
        _spreadCalculator = spreadCalculator;
    }
    private async Task<List<LeaderboardModel>> InteralLeaderboard(int leagueId, long seasonYear) {
        var leaderboard = new List<LeaderboardModel>();
        using var db = _dbContextFactory.CreateDbContext();
        if (db == null) {
            Log.Error("Database context is not initialized.");
            return leaderboard;
        }
        Log.Information("Loading Scoreboard {LeagueId}", leagueId);
        try {
            var allUsers = await db.LeagueUserMapping
                    .Include(lum => lum.User) // Ensure the User entity is included
                    .ToListAsync();
            //Log.Information("{@AllUsers}", allUsers);

            var leagueUsers = await db.LeagueUserMapping
                .Where(lum => lum.LeagueId == leagueId)
                    .Include(lum => lum.User) // Ensure the User entity is included
                .ToListAsync();

            //Log.Information("{@LeagueUsers}", leagueUsers);
            var userScores = await db.NFLScores
                .Where(score => score.Season == seasonYear)
                .ToListAsync();

            if (userScores == null) {
                Log.Error("User scores not found.");
                return leaderboard;
            }

            var maxWeek = userScores.Max(x => x.NFLWeek);
            Log.Information("Season {MaxWeek}", maxWeek);
            //Log.Information("{@Spreads}", spreads);
            var leagueInfo = await db.LeagueJuiceMapping
                .Where(li => li.LeagueId == leagueId)
                .FirstOrDefaultAsync();
            var leagueInfoDos = await db.LeagueUserMapping.ToListAsync();

            if (leagueInfo == null) {
                Log.Error("League info not found.");
                return leaderboard;
            }

            //Log.Information("{@LeagueInfo}", leagueInfo);
            foreach (var user in leagueUsers) {
                var userPoints = new LeaderboardModel();
                userPoints.WeekResults = new LeaderboardWeekResults[maxWeek];
                userPoints.User = user.User;
                //Log.Information("User Details {Id} {Email} {UserName} {NickName}", userPoints.User.Id, userPoints.User.Email, userPoints.User.UserName, userPoints.User.NickName);
                // Regular Season
                for (int week = 1; week <= maxWeek; week++) {
                    var isPostSeason = week > 18;
                    LeaderboardWeekResults weekResult;
                    if (!isPostSeason) {
                        weekResult = await CalculateRegularSeasonPicks(leagueId, seasonYear, db, userScores, user, week);
                    }
                    else {
                        weekResult = await CalculatePostSeasonPicks(leagueId, seasonYear, db, userScores, user, week);
                    }
                    userPoints.WeekResults[week - 1] = weekResult;
                }
                //Log.Information("{@Row}", row);
                leaderboard.Add(userPoints);
            }

            // Calc Totals
            leaderboard = await CalculateUserTotals(leaderboard, leagueId, seasonYear, maxWeek);
        }
        catch (Exception ex) {
            Log.Error(ex, "Error loading leaderboard");
            return leaderboard;
        }
        return leaderboard;
    }
    private async Task<LeaderboardWeekResults> CalculatePostSeasonPicks(int leagueId, long seasonYear, ApplicationDbContext db, List<NFLScores> userScores, LeagueUserMapping user, int week) {
        // Normalize Week from 18 to basis 0
        //Log.Information("Calculating Post Season Picks {Week}", week);
        var postSeasonWeek = week - 18;
        var weekResult = new LeaderboardWeekResults();
        weekResult.Week = week;
        var userPicks = await db.NFLPostSeasonPicks
            .Where(pick => pick.UserId == user.UserId && pick.Season == seasonYear && pick.NFLWeek == postSeasonWeek)
            .ToListAsync();

        if (userPicks.Count < GameHelpers.GetRequiredPicks(week)) {
            Log.Information("Missing PostSeason Picks {Week} {Count} {Required}", week, userPicks.Count, GameHelpers.GetRequiredPicks(week));
            weekResult.WeekResult = WeekResult.MissingPicks;
        }
        else {
            bool allPicksBeatSpread = userPicks.All(pick => {
                _spreadCalculator.Configure(leagueId, week, (int)seasonYear);
                if (!_spreadCalculator.DoOddsExist()) return false;
                var score = userScores.FirstOrDefault(s => s.NFLWeek == week);
                if (pick.Pick == PickType.Spread) {
                    var spread = _spreadCalculator.GetSpread(pick.Team);
                    if (score is null || spread is null) return false;
                    var isHome = score.HomeTeam == pick.Team;
                    if (isHome)
                        return (score.HomeTeamScore - score.AwayTeamScore) > spread;
                    else return (score.AwayTeamScore - score.HomeTeamScore) > spread;
                }
                else if (pick.Pick == PickType.Over) {
                    var overUnder = _spreadCalculator.GetOverUnder(pick.Team, pick.Pick);
                    if (score is null || overUnder is null) return false;
                    if ((score.HomeTeamScore + score.AwayTeamScore) > overUnder)
                        return true;
                    else return false;
                }
                else if (pick.Pick == PickType.Under) {
                    var overUnder = _spreadCalculator.GetOverUnder(pick.Team, pick.Pick);
                    if (score is null || overUnder is null) return false;
                    if ((score.HomeTeamScore + score.AwayTeamScore) < overUnder)
                        return true;
                    else
                        return false;
                }
                return false;
            });
            weekResult.WeekResult = allPicksBeatSpread ? WeekResult.Won : WeekResult.Lost;
        }

        return weekResult;
    }
    private async Task<LeaderboardWeekResults> CalculateRegularSeasonPicks(int leagueId, long seasonYear, ApplicationDbContext db, List<NFLScores> userScores, LeagueUserMapping user, int week) {
        //Log.Information("Calculating Season Picks {Week}", week);
        var weekResult = new LeaderboardWeekResults();
        weekResult.Week = week;
        var userPicks = await db.NFLPicks
            .Where(pick => pick.UserId == user.UserId && pick.Season == seasonYear && pick.NFLWeek == week)
            .ToListAsync();

        if (userPicks.Count < GameHelpers.GetESPNRequiredPicks(week, false)) {
            Log.Information("Missing Picks {Week} {Count} {Required}", week, userPicks.Count, GameHelpers.GetESPNRequiredPicks(week, false));
            weekResult.WeekResult = WeekResult.MissingPicks;
        }
        else {
            bool allPicksBeatSpread = userPicks.All(pick => {
                _spreadCalculator.Configure(leagueId, week, (int)seasonYear);
                if (!_spreadCalculator.DoOddsExist()) return false;
                var score = userScores.FirstOrDefault(s => s.NFLWeek == week);
                var spread = _spreadCalculator.GetSpread(pick.Team);
                if (score is null || spread is null) return false;
                var isHome = score.HomeTeam == pick.Team;
                if (isHome)
                    return (score.HomeTeamScore - score.AwayTeamScore) > spread;
                else return (score.AwayTeamScore - score.HomeTeamScore) > spread;

            });
            weekResult.WeekResult = allPicksBeatSpread ? WeekResult.Won : WeekResult.Lost;
        }

        return weekResult;
    }

    public async Task<List<LeaderboardModel>> BuildLeaderboard(int leagueId, long seasonYear) {
        if (leagueId == 0) {
            Log.Error("League ID is not set.");
            return new List<LeaderboardModel>();
        }
        return await _memory.GetOrCreateAsync($"leaderboard:{leagueId}", async (option) => {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            return await InteralLeaderboard(leagueId, seasonYear);
        });
    }
    public async Task<List<LeaderboardModel>> CalculateUserTotals(List<LeaderboardModel> leaderboard, int leagueId, long seasonYear, int maxWeek) {
        Log.Information("Loading User Totals");
        using var db = _dbContextFactory.CreateDbContext();
        var baseWeeklyCost = await db.LeagueJuiceMapping
            .Where(ljm => ljm.LeagueId == leagueId && ljm.Season == seasonYear)
            .Select(ljm => ljm.WeeklyCost)
            .FirstOrDefaultAsync();

        if (baseWeeklyCost == 0) {
            Log.Error("Base weekly cost not found.");
            return leaderboard;
        }
        var userTotals = new Dictionary<string, decimal>();
        var currentWeeklyCost = baseWeeklyCost;

        for (int week = 1; week <= maxWeek; week++) {
            var winners = new List<string>();
            bool allUsersWon = true;

            foreach (var result in leaderboard) {
                var resultWeek = result.WeekResults.FirstOrDefault(w => w.Week == week);
                var userId = result.User.Id;
                if (resultWeek.WeekResult == WeekResult.Won) {
                    winners.Add(userId);
                }
                else {
                    allUsersWon = false;
                }
            }
            // Nobody winning is the same thing as everyone losing
            if (winners.Count == 0) {
                Log.Information("No winners week {Week} {Juice}", week, currentWeeklyCost);
                allUsersWon = false;
            }
            // Fill in
            if (!allUsersWon) {
                foreach (var result in leaderboard) {
                    var userId = result.User.Id;
                    if (result.WeekResults[week - 1].WeekResult != WeekResult.Won) {
                        //Log.Information("User {User} lost week {Week} {Count} {Cost}", userId, week, winners.Count, currentWeeklyCost);
                        result.WeekResults[week - 1].Score = -(winners.Count * currentWeeklyCost);
                    }
                    else {
                        Log.Information("User {User} won week {Week} {Count} {Winnings}", userId, week, winners.Count, currentWeeklyCost);
                        result.WeekResults[week - 1].Score = winners.Count * currentWeeklyCost;
                    }
                }
                currentWeeklyCost = baseWeeklyCost;
            }
            // Double the cost for the next week if all users won this week
            else {
                currentWeeklyCost += baseWeeklyCost;
                Log.Information("All users won or lost week {Week} Doubling {Juice}", week, currentWeeklyCost);
                foreach (var result in leaderboard) {
                    result.WeekResults[week - 1].Score = 0;
                }
            }
        }
        foreach (var result in leaderboard) {
            result.Total = result.WeekResults.Sum(w => w.Score);
        }
        return leaderboard;
    }
}