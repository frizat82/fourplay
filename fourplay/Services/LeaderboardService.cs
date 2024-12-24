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
    public async Task<DataTable> InteralLeaderboard(int leagueId) {
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

            var scores = await _espn!.GetScores();
            if (scores is null)
                return dataTable;
            var allUsers = await db.LeagueUserMapping
                    .Include(lum => lum.User) // Ensure the User entity is included
                    .ToListAsync();
            Log.Information("{@AllUsers}", allUsers);

            var leagueUsers = await db.LeagueUserMapping
                .Where(lum => lum.LeagueId == leagueId)
                    .Include(lum => lum.User) // Ensure the User entity is included
                .ToListAsync();

            Log.Information("{@LeagueUsers}", leagueUsers);
            var userScores = await db.NFLScores
                .Where(score => score.Season == scores.Season.Year)
                .ToListAsync();

            if (userScores == null) {
                Log.Error("User scores not found.");
                return dataTable;
            }

            var spreads = await db.NFLSpreads.Where(spread => spread.Season == scores.Season.Year).ToListAsync();

            //Log.Information("{@Spreads}", spreads);
            var leagueInfo = await db.LeagueJuiceMapping
                .Where(li => li.LeagueId == leagueId)
                .FirstOrDefaultAsync();
            var leagueInfoDos = await db.LeagueUserMapping.ToListAsync();

            if (leagueInfo == null) {
                Log.Error("League info not found.");
                return dataTable;
            }

            Log.Information("{@LeagueInfo}", leagueInfo);
            dataTable = new DataTable();
            dataTable.Columns.Add("User");

            for (int week = 1; week <= 16; week++) {
                dataTable.Columns.Add($"Week {week}");
            }

            foreach (var user in leagueUsers) {
                DataRow row = dataTable.NewRow();
                row["User"] = user.User.Email;

                for (int week = 1; week <= 16; week++) {
                    var userPicks = await db.NFLPicks
                        .Where(pick => pick.UserId == user.UserId && pick.Season == scores.Season.Year && pick.NFLWeek == week)
                        .ToListAsync();

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
                    row[$"Week {week}"] = allPicksBeatSpread ? "Yes" : "No";
                }
                //Log.Information("{@Row}", row);
                dataTable.Rows.Add(row);
            }
        }
        catch (Exception ex) {
            Log.Error(ex, "Error loading leaderboard");
            return dataTable ?? new DataTable();
        }
        return dataTable;
    }

    public async Task<DataTable> BuildLeaderboard(int leagueId) {
        if (leagueId == 0) {
            Log.Error("League ID is not set.");
            return new DataTable();
        }
        return await _memory.GetOrCreateAsync($"leaderboard:{leagueId}", async (option) => {
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            return await InteralLeaderboard(leagueId);
        });
    }
}