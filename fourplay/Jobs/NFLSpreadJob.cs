using fourplay.Data;
using Quartz;
using Serilog;

[DisallowConcurrentExecution]
public class NFLSpreadJob : IJob {
    private readonly ISportslineOddsService _sportsOdds;
    private readonly IESPNApiService _espn;
    private readonly ApplicationDbContext _context;
    public NFLSpreadJob(ISportslineOddsService sportsOdds, IESPNApiService espn, ApplicationDbContext context) {
        _sportsOdds = sportsOdds;
        _context = context;
        _espn = espn;
    }
    public async Task Execute(IJobExecutionContext context) {
        // TODO: Implement logic to grab NFL spreads
        Log.Information("Grabbing NFL Spreads at " + DateTime.Now);
        var scoreboard = await _espn.GetScores();
        var newGames = scoreboard?.Events.SelectMany(x => x.Competitions).Where(y => y.Status.Type.Name == TypeName.StatusScheduled);
        var odds = await _sportsOdds.GetOdds();
        var spreads = new List<NFLSpreads>();
        if (odds?.Data != null) {
            foreach (var result in odds?.Data?.Odds?.OddsCompetitions) {
                var spread = new NFLSpreads();
                //spread.Id = Guid.NewGuid();
                var ht = result.HomeTeam;
                var at = result.AwayTeam;
                if (Helpers.NFLTeamAbbrMapping.ContainsKey(ht.Abbr))
                    spread.HomeTeam = Helpers.NFLTeamAbbrMapping[ht.Abbr];
                else
                    spread.HomeTeam = ht.Abbr;
                if (Helpers.NFLTeamAbbrMapping.ContainsKey(ht.Abbr))
                    spread.AwayTeam = Helpers.NFLTeamAbbrMapping[ht.Abbr];
                else
                    spread.AwayTeam = at.Abbr;
                spread.GameTime = result.ScheduledTime.UtcDateTime;
                var cleanHomeSpread = result.SportsBookOdds.Consensus.Spread.Home.Value.Replace("+", "");
                var cleanAwaySpread = result.SportsBookOdds.Consensus.Spread.Away.Value.Replace("+", "");
                if (cleanHomeSpread == "FK") {
                    Log.Error("Error");
                }
                if (cleanAwaySpread == "FK") {
                    Log.Error("Error");
                }
                if (!Double.TryParse(cleanHomeSpread, out var parsedSpread))
                    continue;
                spread.HomeTeamSpread = parsedSpread;
                if (!Double.TryParse(cleanAwaySpread, out parsedSpread))
                    continue;
                spread.AwayTeamSpread = parsedSpread;
                var matchedGame = newGames.Where(x => x.Date == spread.GameTime).SelectMany(x => x.Competitors).Where(y => y.HomeAway == HomeAway.Home).FirstOrDefault(z => z.Team.Abbreviation == spread.HomeTeam);
                if (matchedGame is not null) {
                    spread.NFLWeek = (int)scoreboard.Week.Number;
                    spread.Season = (int)scoreboard.Season.Year;
                    var record = _context.NFLSpreads.FirstOrDefault(x => x.Season == spread.Season && x.NFLWeek == spread.NFLWeek && x.HomeTeam == spread.HomeTeam);
                    if (record is null) {
                        spreads.Add(spread);
                    }
                }

            }
        }
        if (spreads.Any()) {
            Log.Information("Load NFL Spreads at " + DateTime.Now);
            await _context.NFLSpreads.AddRangeAsync(spreads);
            await _context.SaveChangesAsync();
        }
        Log.Information("NFL Spreads Complete at " + DateTime.Now);
    }
}

/*
SQLite error (1299): abort at 17 in [INSERT INTO "NFLSpreads" ("AwayTeam", "AwayTeamSpread", "GameTime", "HomeTeam", "HomeTeamSpread", "NFLWeek", "Season")
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)
RETURNING "Id", "DateCreated";]
[14:00:01 ERR] Failed executing DbCommand (0ms) [Parameters=[@p0='?', @p1='?' (DbType = Double), @p2='?' (DbType = DateTime), @p3='?', @p4='?' (DbType = Double), @p5='?' (DbType = Int32), @p6='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
INSERT INTO "NFLSpreads" ("AwayTeam", "AwayTeamSpread", "GameTime", "HomeTeam", "HomeTeamSpread", "NFLWeek", "Season")
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)
RETURNING "Id", "DateCreated";
*/