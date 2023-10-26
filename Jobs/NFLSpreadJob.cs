using fourplay.Data;
using Quartz;
using SportslineOdds;
using System;
using System.Threading.Tasks;

[DisallowConcurrentExecution]
public class NFLSpreadJob : IJob
{
    private readonly ISportslineOddsService _sportsOdds;
    private readonly IESPNApiService _espn;
    private readonly ApplicationDbContext _context;
    public NFLSpreadJob(ISportslineOddsService sportsOdds, IESPNApiService espn, ApplicationDbContext context)
    {
        _sportsOdds = sportsOdds;
        _context = context;
        _espn = espn;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        // TODO: Implement logic to grab NFL spreads
        Console.WriteLine("Grabbing NFL Spreads at " + DateTime.Now);
        var scoreboard = await _espn.GetScores();
        var newGames = scoreboard.Events.SelectMany(x => x.Competitions).Where(y => y.Status.Type.Name == TypeName.StatusScheduled);
        var odds = await _sportsOdds.GetOdds();
        var spreads = new List<NFLSpreads>();
        if (odds?.Data != null)
        {
            foreach (var result in odds?.Data?.Odds?.OddsCompetitions)
            {
                var spread = new NFLSpreads();
                var ht = result.HomeTeam;
                var at = result.AwayTeam;
                spread.HomeTeam = ht.Abbr;
                spread.AwayTeam = at.Abbr;
                spread.GameTime = result.ScheduledTime.UtcDateTime;
                spread.HomeTeamSpread = Double.Parse(result.SportsBookOdds.Consensus.Spread.Home.Value.Replace("+", ""));
                spread.AwayTeamSpread = Double.Parse(result.SportsBookOdds.Consensus.Spread.Away.Value.Replace("+", ""));

                var matchedGame = newGames.Where(x => x.Date == spread.GameTime).SelectMany(x => x.Competitors).Where(y => y.HomeAway == HomeAway.Home).FirstOrDefault(z => z.Team.Abbreviation == spread.HomeTeam);
                if (matchedGame is not null)
                {
                    spread.NFLWeek = (int)scoreboard.Week.Number;
                    spread.Season = (int)scoreboard.Season.Year;
                    var record = _context.NFLSpreads.FirstOrDefault(x => x.Season == spread.Season && x.NFLWeek == spread.NFLWeek && x.HomeTeam == spread.HomeTeam);
                    if (record is null)
                    {
                        spreads.Add(spread);
                    }
                }

            }
        }
        if (spreads.Any())
        {
            await _context.NFLSpreads.AddRangeAsync(spreads);
            await _context.SaveChangesAsync();
        }
    }
}