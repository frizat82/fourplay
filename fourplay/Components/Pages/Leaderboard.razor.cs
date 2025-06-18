using System;
using System.Data;
using Blazored.LocalStorage;
using fourplay.Models;
using fourplay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace fourplay.Components.Pages;
[Authorize]
public partial class Leaderboard : ComponentBase {
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ILeaderboardService _leaderboard { get; set; } = default!;
    private int _leagueId = 0;
    private List<LeaderboardModel> _leaderboardModel = new();
    private bool _loading = true;
    private async Task BuildScoreboard() {
        Log.Information("Loading Scoreboard {LeagueId}", _leagueId);
        _loading = true;
        var scores = await _espn!.GetScores();
        _leaderboardModel = await _leaderboard.BuildLeaderboard(_leagueId, scores!.Season.Year);
        _loading = false;
        await InvokeAsync(StateHasChanged);
    }
    private int GetMaxWeek() => _leaderboardModel.Max(x => x.WeekResults.Max(y => y.Week));
    public Func<DataRow, object> Sort => (DataRow row) => {
        var value = row["Total"].ToString();
        if (value is null || value == "")
            return 0;
        return Double.Parse(value);
    };
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender || _leagueId == 0) {
            try {
                var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
                Log.Information("LeagueId: {LeagueId}", leagueId);
                if (leagueId == 0) {
                    Log.Information("LeagueId is 0");
                    await _localStorage.RemoveItemAsync("leagueId");
                    Navigation.NavigateTo("/leagues");
                }
                _leagueId = leagueId;
                await BuildScoreboard();
            }
            catch (Exception ex) {
                Log.Error(ex, "Error getting leagueId");
                Navigation.NavigateTo("/leagues");
            }
        }
        else if (_leagueId == 0) {
            Log.Information("_leagueId is 0");
            Navigation.NavigateTo("/leagues");
        }
    }
}
