using System.Data;
using System.Threading.Tasks;
using fourplay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

public class LeaderboardService : ILeaderboardService {
    private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; }
    private IESPNApiService? _espn { get; set; }
    private readonly IMemoryCache _memory;
    public LeaderboardService(IESPNApiService espnService, IMemoryCache memoryCache, IDbContextFactory<ApplicationDbContext> dbFactory) {
        _memory = memoryCache;
        _dbContextFactory = dbFactory;
        _espn = espnService;
    }
    public async Task<DataTable> InteralLeaderboard(int leagueId, long seasonYear) {
        var dataTable = new DataTable();
        using var db = _dbContextFactory.CreateDbContext();
        if (db == null) {
            Log.Error("Database context is not initialized.");
            return dataTable;
        }
        Log.Information("Loading Scoreboard {LeagueId}", leagueId);
        try {
            if (_espn == null) {
                Log.Error("ESPN API service is not initialized.");
                return dataTable;
            }

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
                return dataTable;
            }

            var spreads = await db.NFLSpreads.Where(spread => spread.Season == seasonYear).ToListAsync();

            //Log.Information("{@Spreads}", spreads);
            var leagueInfo = await db.LeagueJuiceMapping
                .Where(li => li.LeagueId == leagueId)
                .FirstOrDefaultAsync();
            var leagueInfoDos = await db.LeagueUserMapping.ToListAsync();

            if (leagueInfo == null) {
                Log.Error("League info not found.");
                return dataTable;
            }

            //Log.Information("{@LeagueInfo}", leagueInfo);
            dataTable = new DataTable();
            dataTable.Columns.Add("User");
            dataTable.Columns.Add("Total");
            for (int week = 1; week <= 16; week++) {
                dataTable.Columns.Add($"Week {week}");
            }

            foreach (var user in leagueUsers) {
                DataRow row = dataTable.NewRow();
                row["User"] = user.User.Email;

                for (int week = 1; week <= 16; week++) {
                    var userPicks = await db.NFLPicks
                        .Where(pick => pick.UserId == user.UserId && pick.Season == seasonYear && pick.NFLWeek == week)
                        .ToListAsync();

                    if (userPicks.Count < 4) {
                        row[$"Week {week}"] = false;
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
                        row[$"Week {week}"] = allPicksBeatSpread ? true : false;
                    }
                }
                //Log.Information("{@Row}", row);
                dataTable.Rows.Add(row);
            }
            // Calc Totals
            dataTable = await CalculateUserTotals(dataTable, leagueId, seasonYear);
        }
        catch (Exception ex) {
            Log.Error(ex, "Error loading leaderboard");
            return dataTable ?? new DataTable();
        }
        return dataTable;
    }

    public async Task<DataTable> BuildLeaderboard(int leagueId, long seasonYear) {
        if (leagueId == 0) {
            Log.Error("League ID is not set.");
            return new DataTable();
        }
        return await _memory.GetOrCreateAsync($"leaderboard:{leagueId}", async (option) => {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            return await InteralLeaderboard(leagueId, seasonYear);
        });
    }
    public async Task<DataTable> CalculateUserTotals(DataTable dataTable, int leagueId, long seasonYear) {
        Log.Information("Loading User Totals");
        using var db = _dbContextFactory.CreateDbContext();
        var baseWeeklyCost = await db.LeagueJuiceMapping
            .Where(ljm => ljm.LeagueId == leagueId && ljm.Season == seasonYear)
            .Select(ljm => ljm.WeeklyCost)
            .FirstOrDefaultAsync();

        if (baseWeeklyCost == 0) {
            Log.Error("Base weekly cost not found.");
            return dataTable;
        }
        var userTotals = new Dictionary<string, decimal>();
        var currentWeeklyCost = baseWeeklyCost;

        for (int week = 1; week <= 16; week++) {
            var weekColumn = $"Week {week}";
            var winners = new List<string>();
            bool allUsersWon = true;

            foreach (DataRow row in dataTable.Rows) {
                var userEmail = row["User"].ToString();
                if (userEmail != null && row[weekColumn].ToString() == "True") {
                    winners.Add(userEmail);
                }
                else {
                    allUsersWon = false;
                    Log.Information("User {User} did not win {Week}", userEmail, week);
                }
            }
            // Nobody winning is the same thing as everyone losing
            if (winners.Count == 0) {
                allUsersWon = false;
            }
            // Fill in
            if (!allUsersWon) {
                foreach (DataRow row in dataTable.Rows) {
                    var userEmail = row["User"].ToString();
                    if (userEmail != null && row[weekColumn].ToString() == "False") {
                        if (!userTotals.ContainsKey(userEmail)) {
                            userTotals[userEmail] = 0;
                        }
                        Log.Information("User {User} lost week {Week} {Count} {Cost}", userEmail, week, winners.Count, currentWeeklyCost);
                        userTotals[userEmail] += winners.Count * currentWeeklyCost;
                    }
                }
                currentWeeklyCost = baseWeeklyCost;
            }
            // Double the cost for the next week if all users won this week
            else {
                currentWeeklyCost += baseWeeklyCost;
                Log.Information("All users won week {Week} Doubling {Juice}", week, currentWeeklyCost);
            }
        }
        foreach (DataRow row in dataTable.Rows) {
            var userEmail = row["User"].ToString();
            if (userTotals.ContainsKey(userEmail!))
                row["Total"] = userTotals[userEmail!];
        }
        return dataTable;
    }
}