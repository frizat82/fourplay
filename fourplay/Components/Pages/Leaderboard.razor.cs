using System;
using System.Data;
using Blazored.LocalStorage;
using fourplay.Data;
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
    private DataTable _dataTable = new();
    private bool _loading = true;

    private async Task BuildScoreboard() {
        Log.Information("Loading Scoreboard {LeagueId}", _leagueId);
        _loading = true;
        _dataTable = await _leaderboard.BuildLeaderboard(_leagueId);
        _loading = false;
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            try {
                var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
                if (leagueId == 0) {
                    await _localStorage.RemoveItemAsync("leagueId");
                    Navigation.NavigateTo("/leagues");
                }
                _leagueId = leagueId;
                await BuildScoreboard();
            }
            catch (Exception) {
                Navigation.NavigateTo("/leagues");
            }
        }
        else if (_leagueId == 0)
            Navigation.NavigateTo("/leagues");
    }
}
