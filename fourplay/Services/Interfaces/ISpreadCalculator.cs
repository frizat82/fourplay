using fourplay.Models.Enum;
using fourplay.Data;

namespace fourplay.Services.Interfaces;
public interface ISpreadCalculator {
    public bool DoOddsExist();
    public void Configure(int leagueId, int week, int season);
    double? GetOverUnder(string teamAbbr, PickType pickType);
    double? GetSpread(string teamAbbr);
    double CalculateLeagueSpread();
    double? GetSpreadFromAbbreviation(string teamAbbr);
    double? GetOverUnderFromAbbreviation(string teamAbbr);
    LeagueJuiceMapping GetLeagueJuice();
}