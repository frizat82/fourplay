using System.Data;

public interface ILeaderboardService {
    public Task<DataTable> BuildLeaderboard(int leagueId, long seasonYear);
    public Task<DataTable> CalculateUserTotals(DataTable dataTAble, int leagueId, long seasonYear);
}