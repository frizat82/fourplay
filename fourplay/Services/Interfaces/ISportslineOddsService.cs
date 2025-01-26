namespace fourplay.Services.Interfaces;
using fourplay.Data;

public interface ISportslineOddsService {
    public Task<SportslineOddsData?> GetOdds();
}