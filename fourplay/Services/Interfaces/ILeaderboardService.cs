namespace fourplay.Services.Interfaces;

using fourplay.Models;
using System.Data;

public interface ILeaderboardService {
    public Task<List<LeaderboardModel>> BuildLeaderboard(int leagueId, long seasonYear);
    public Task<List<LeaderboardModel>> CalculateUserTotals(List<LeaderboardModel> leaderboard, int leagueId, long seasonYear, int maxWeek);
}