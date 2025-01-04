namespace fourplay.Services.Interfaces;
using SportslineOdds;

public interface ISportslineOddsService {
    public Task<SportslineOddsData?> GetOdds();
}