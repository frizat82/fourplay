using fourplay.Data;
using fourplay.Models;
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
        var postSeasonScoreList = new List<NFLScores>();
        var scoreList = new List<NFLScores>();
        for (var i = -2; i < 2; i++) {
            for (var j = 1; j < 18; j++) {
                // TODO: how do i know the year?
                var scores = await _espn.GetWeekScores(j, DateTime.Now.AddYears(i).Year);
                if (scores is null)
                    break;
                var results = scores.Events.SelectMany(x => x.Competitions, (x, y) => new CompetitionBySeason { Season = x.Season, Competition = y }).Where(y => y.Competition.Status.Type.Name == TypeName.StatusFinal);
                if (results.Any()) {
                    scoreList.AddRange(ParseCompetition(results, j));
                }
                else
                    break;
            }
            for (var j = 1; j < 6; j++) {
                // TODO: how do i know the year?
                var scores = await _espn.GetWeekScores(j, DateTime.Now.AddYears(i).Year, true);
                if (scores is null)
                    break;
                var results = scores.Events.SelectMany(x => x.Competitions, (x, y) => new CompetitionBySeason { Season = x.Season, Competition = y }).Where(y => y.Competition.Status.Type.Name == TypeName.StatusFinal);
                if (results.Any()) {
                    postSeasonScoreList.AddRange(ParseCompetition(results, j));
                }
                else
                    break;
            }
        }
        if (scoreList.Any()) {
            await _context.NFLScores.AddRangeAsync(scoreList);
            await _context.SaveChangesAsync();
        }
        if (postSeasonScoreList.Any()) {
            await _context.NFLPostSeasonScores.AddRangeAsync(postSeasonScoreList);
            await _context.SaveChangesAsync();
        }

    }
    private List<NFLScores> ParseCompetition(IEnumerable<CompetitionBySeason> competitions, int week) {
        var scoreList = new List<NFLScores>();
        foreach (var result in competitions) {
            if (result.Competition.Status.Type.Name == TypeName.StatusFinal) {
                var dbScore = new NFLScores();
                var ht = result.Competition.Competitors.Where(x => x.HomeAway == HomeAway.Home).First();
                var at = result.Competition.Competitors.Where(x => x.HomeAway == HomeAway.Away).First();
                //dbScore.Id = Guid.NewGuid();
                dbScore.HomeTeam = ht.Team.Abbreviation;
                dbScore.AwayTeam = at.Team.Abbreviation;
                dbScore.HomeTeamScore = ht.Score;
                dbScore.AwayTeamScore = at.Score;
                dbScore.NFLWeek = week;
                dbScore.Season = (int)result.Season.Year;
                dbScore.GameTime = result.Competition.Date.UtcDateTime;
                var record = _context.NFLScores.FirstOrDefault(x => x.Season == dbScore.Season && x.NFLWeek == dbScore.NFLWeek && x.HomeTeam == dbScore.HomeTeam);
                if (record is null)
                    scoreList.Add(dbScore);
            }
        }
        return scoreList;
    }
}
