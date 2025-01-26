using fourplay.Data;
using fourplay.Models.Enum;
using fourplay.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace fourplay.Services;
public class SpreadCalculator : ISpreadCalculator {
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private List<NFLSpreads> _odds = new();
    private bool _isPostSeason = false;
    private long _week = 0;
    private long _season = 0;
    private int _leagueId = 0;

    public SpreadCalculator(IDbContextFactory<ApplicationDbContext> dbContextFactory) {
        _dbContextFactory = dbContextFactory;

    }
    public void Configure(int leagueId, int week, int season, bool isPostSeason) {
        _isPostSeason = isPostSeason;
        _week = week;
        _season = season;
        _leagueId = leagueId;
        using var db = _dbContextFactory.CreateDbContext();
        if (!isPostSeason)
            _odds = db.NFLSpreads.Where(x => x.Season == _season && x.NFLWeek == _week).ToList();
        else
            _odds = db.NFLPostSeasonSpreads.Where(x => x.Season == _season && x.NFLWeek == _week).ToList();
    }
    public bool DoOddsExist() {
        return _odds!.Count > 0;
    }

    public double? GetOverUnder(string teamAbbr, PickType pickType) {
        //TODO: Add Caching
        var spread = GetOverUnderFromAbbreviation(teamAbbr);
        if (pickType == PickType.Over)
            return spread - CalculateLeagueSpread();
        return spread + CalculateLeagueSpread();
    }

    public double? GetSpread(string teamAbbr) {
        //TODO: Add Caching
        var spread = GetSpreadFromAbbreviation(teamAbbr);
        return spread + CalculateLeagueSpread();
    }

    public double CalculateLeagueSpread() {
        var baseSpread = GetLeagueJuice();
        if (!_isPostSeason)
            return baseSpread.Juice;
        if (_week < 3)
            return baseSpread.JuiceDivisonal;
        if (_week == 3)
            return baseSpread.JuiceConference;
        return 0;
    }

    public double? GetSpreadFromAbbreviation(string teamAbbr) {
        var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
        if (spread is not null)
            return spread.HomeTeamSpread;
        spread = _odds.First(x => x.AwayTeam == teamAbbr);
        return spread.AwayTeamSpread;
    }

    public double? GetOverUnderFromAbbreviation(string teamAbbr) {
        var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
        if (spread is not null)
            return spread.OverUnder;
        spread = _odds.First(x => x.AwayTeam == teamAbbr);
        return spread.OverUnder;
    }

    public LeagueJuiceMapping GetLeagueJuice() {
        using var db = _dbContextFactory.CreateDbContext();
        var leagueSpread = db.LeagueJuiceMapping.First(x => x.Id == _leagueId && x.Season == _season);
        return leagueSpread;

    }
}