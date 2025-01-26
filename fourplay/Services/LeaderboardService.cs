using System.Data;
using fourplay.Data;
using fourplay.Models;
using fourplay.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using fourplay.Models.Enum;
namespace fourplay.Services;
public class LeaderboardService : ILeaderboardService {
    private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; }
    private readonly IMemoryCache _memory;
    public LeaderboardService(IMemoryCache memoryCache, IDbContextFactory<ApplicationDbContext> dbFactory) {
        _memory = memoryCache;
        _dbContextFactory = dbFactory;
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

            var spreads = await db.NFLSpreads.Where(spread => spread.Season == seasonYear).ToListAsync();

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
                userPoints.WeekResults = new LeaderboardWeekResults[16];
                userPoints.User = user.User;
                // Regular Season
                for (int week = 1; week <= 16; week++) {
                    var weekResult = new LeaderboardWeekResults();
                    weekResult.Week = week;
                    var userPicks = await db.NFLPicks
                        .Where(pick => pick.UserId == user.UserId && pick.Season == seasonYear && pick.NFLWeek == week)
                        .ToListAsync();

                    if (userPicks.Count < 4) {
                        weekResult.WeekResult = WeekResult.MissingPicks;
                    }
                    else {
                        bool allPicksBeatSpread = userPicks.All(pick => {
                            var score = userScores.FirstOrDefault(s => s.NFLWeek == week);
                            var homeTeamSpread = spreads.FirstOrDefault(s => s.NFLWeek == week && s.HomeTeam == pick.Team);
                            var awayTeamSpread = spreads.FirstOrDefault(s => s.NFLWeek == week && s.AwayTeam == pick.Team);
                            if (score == null || homeTeamSpread == null || awayTeamSpread == null) return false;
                            if (homeTeamSpread is not null) {
                                var adjustedSpread = homeTeamSpread.HomeTeamSpread + leagueInfo!.Juice;
                                return (score.HomeTeamScore - score.AwayTeamScore) > adjustedSpread;
                            }
                            else {
                                var adjustedSpread = awayTeamSpread.AwayTeamSpread + leagueInfo!.Juice;
                                return (score.AwayTeamScore - score.HomeTeamScore) > adjustedSpread;
                            }
                        });
                        weekResult.WeekResult = allPicksBeatSpread ? WeekResult.Won : WeekResult.Lost;
                    }
                    userPoints.WeekResults[week - 1] = weekResult;
                }
                //Log.Information("{@Row}", row);
                leaderboard.Add(userPoints);
            }

            // Calc Totals
            leaderboard = await CalculateUserTotals(leaderboard, leagueId, seasonYear);
        }
        catch (Exception ex) {
            Log.Error(ex, "Error loading leaderboard");
            return leaderboard;
        }
        return leaderboard;
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
    public async Task<List<LeaderboardModel>> CalculateUserTotals(List<LeaderboardModel> leaderboard, int leagueId, long seasonYear) {
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

        for (int week = 1; week <= 16; week++) {
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
                allUsersWon = false;
            }
            // Fill in
            if (!allUsersWon) {
                foreach (var result in leaderboard) {
                    var userId = result.User.Id;
                    if (result.WeekResults[week - 1].WeekResult != WeekResult.Won) {
                        Log.Information("User {User} lost week {Week} {Count} {Cost}", userId, week, winners.Count, currentWeeklyCost);
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
                Log.Information("All users won week {Week} Doubling {Juice}", week, currentWeeklyCost);
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