namespace fourplay.Services.Interfaces;
using System.Data;

public interface ILeaderboardService {
    public Task<DataTable> BuildLeaderboard(int leagueId, long seasonYear);
    public Task<DataTable> CalculateUserTotals(DataTable dataTable, int leagueId, long seasonYear);
}