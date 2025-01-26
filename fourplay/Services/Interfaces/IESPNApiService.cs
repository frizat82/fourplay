namespace fourplay.Services.Interfaces;
using fourplay.Data;
public interface IESPNApiService {
    public Task<ESPNScores?> GetWeekScores(int week, int year, bool postSeason = false);
    /*
    public Task<ESPNApiNFLSeasonDetail?> GetSeasonDetail(int year);
    */
    //public Task<ESPNApiNFLSeasons?> GetSeasons();

    public Task<ESPNScores?> GetScores();
}