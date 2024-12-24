using System.Data;

public interface ILeaderboardService {
    public Task<DataTable> BuildLeaderboard(int leagueId);
}