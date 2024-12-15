using fourplay.Data;
using Quartz;
using Serilog;

namespace fourplay.Jobs;
[DisallowConcurrentExecution]
public class NFLScoresJob : IJob {
    private readonly IESPNApiService _espn;
    private readonly ApplicationDbContext _context;
    public NFLScoresJob(IESPNApiService espn, ApplicationDbContext context) {
        _espn = espn;
        _context = context;
    }
    public async Task Execute(IJobExecutionContext context) {
        // TODO: Implement logic to grab NFL spreads
        Log.Information("Grabbing NFL scores at " + DateTime.Now);

        var scoreList = new List<NFLScores>();
        for (var i = -2; i < 2; i++)
            for (var j = 1; j < 17; j++) {
                // TODO: how do i know the year?
                var scores = await _espn.GetWeekScores(j, DateTime.Now.AddYears(i).Year);
                if (scores is null)
                    break;
                var results = scores.Events.SelectMany(x => x.Competitions).Where(y => y.Status.Type.Name == TypeName.StatusFinal);
                if (results.Any()) {
                    foreach (var result in results) {
                        if (result.Status.Type.Name == TypeName.StatusFinal) {
                            var dbScore = new NFLScores();
                            var ht = result.Competitors.Where(x => x.HomeAway == HomeAway.Home).First();
                            var at = result.Competitors.Where(x => x.HomeAway == HomeAway.Away).First();
                            //dbScore.Id = Guid.NewGuid();
                            dbScore.HomeTeam = ht.Team.Abbreviation;
                            dbScore.AwayTeam = at.Team.Abbreviation;
                            dbScore.HomeTeamScore = ht.Score;
                            dbScore.AwayTeamScore = at.Score;
                            dbScore.NFLWeek = j;
                            dbScore.Season = DateTime.Now.AddYears(i).Year;
                            dbScore.GameTime = result.Date.UtcDateTime;
                            var record = _context.NFLScores.FirstOrDefault(x => x.Season == dbScore.Season && x.NFLWeek == dbScore.NFLWeek && x.HomeTeam == dbScore.HomeTeam);
                            if (record is null)
                                scoreList.Add(dbScore);
                        }

                    }
                }
                else
                    break;
            }
        if (scoreList.Any()) {
            await _context.NFLScores.AddRangeAsync(scoreList);
            await _context.SaveChangesAsync();
        }

    }
}