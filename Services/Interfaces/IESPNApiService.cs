public interface IESPNApiService
{
    public Task<ESPNScores?> GetWeekScores(int week, int year);
    /*
    public Task<ESPNApiNFLSeasonDetail?> GetSeasonDetail(int year);
    public Task<ESPNApiNFLSeasons?> GetSeasons();
    */
    public Task<ESPNScores?> GetScores();
}