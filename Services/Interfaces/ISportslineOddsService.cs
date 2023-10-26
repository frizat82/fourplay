using SportslineOdds;

public interface ISportslineOddsService
{
    public Task<SportslineOddsData?> GetOdds();
}