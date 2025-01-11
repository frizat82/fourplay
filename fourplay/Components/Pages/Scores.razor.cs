using Microsoft.AspNetCore.Authorization;
using fourplay.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NodaTime;
using Microsoft.EntityFrameworkCore;
using Blazored.LocalStorage;
using fourplay.Helpers;
using Serilog;
using fourplay.Services.Interfaces;

namespace fourplay.Components.Pages;
[Authorize]
public partial class Scores : ComponentBase, IDisposable {
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private ApplicationDbContext? _db { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private ESPNScores? _scores = null;
    private Dictionary<string, List<string>> _userPicks = new();
    private System.Timers.Timer _timer = new();
    private List<NFLSpreads>? _odds = null;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    private int _leagueId = 0;
    [Inject] IDialogService _dialogService { get; set; } = default!;
    private bool _loading = true;
    private bool _isPostSeason = false;
    protected override async Task OnInitializedAsync() {
        _loading = true;
        _scores = await _espn.GetScores();
        _odds = await _db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToListAsync();
        await RunTimer();
        _timer.Elapsed += TimeElapsed;
        _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        if (_scores!.Season.Type == (int)TypeOfSeason.PostSeason)
            _isPostSeason = true;
        foreach (var scoreEvent in _scores?.Events) {
            foreach (var competition in scoreEvent.Competitions.OrderBy(x => x.Competitors[1].Team.Abbreviation)) {
                if (GameHelpers.IsGameStarted(competition)) {
                    _userPicks.Add(GameHelpers.GetHomeTeamAbbr(competition), await GetUserPicks(GameHelpers.GetHomeTeamAbbr(competition)));
                    _userPicks.Add(GameHelpers.GetAwayTeamAbbr(competition), await GetUserPicks(GameHelpers.GetAwayTeamAbbr(competition)));
                }
                else {
                    _userPicks.Add(GameHelpers.GetHomeTeamAbbr(competition), new List<string>());
                    _userPicks.Add(GameHelpers.GetAwayTeamAbbr(competition), new List<string>());
                }
            }
        }
        _loading = false;
    }
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
                await InvokeAsync(StateHasChanged);
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
    public string GetIcon(Competition competition, Competitor baseTeam, Competitor compareTeam) {
        var spread = GetSpread(baseTeam.Team.Abbreviation) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Icons.Material.Filled.GppGood;
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Icons.Material.Filled.GppGood;
        else
            return Icons.Material.Filled.GppBad;
    }
    public Color GetColor(Competition competition, Competitor baseTeam, Competitor compareTeam) {
        var spread = GetSpread(baseTeam.Team.Abbreviation) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Color.Success;
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Color.Success;
        else
            return Color.Error;
    }
    public async Task<List<string>> GetUserPicks(string teamAbbr) {
        var picks = await _db!.NFLPicks.Where(x => x.LeagueId == _leagueId && x.Season == _scores!.Season.Year
    && x.NFLWeek == _scores.Week.Number && x.Team == teamAbbr).Select(y => y.User.NormalizedUserName).ToListAsync();
        if (picks is null) {
            return new List<string>();
        }
        return picks!;
    }

    public int GetUserPicks(Competition competition, string teamAbbr) {
        if (!GameHelpers.IsGameStarted(competition)) return 0;
        return _userPicks[teamAbbr].Count!;
    }
    private void TimeElapsed(object? sender, System.Timers.ElapsedEventArgs e) => RunTimer();
    private async Task ShowDialog(string teamAbbr, string logo) {
        var usrNames = _userPicks![teamAbbr];
        if (usrNames.Count > 0) {
            var parameters = new DialogParameters<PickDialog> {
            { x => x.UserNames, usrNames! },
            { x => x.TeamAbbr, teamAbbr},
            {x => x.Logo, logo}};
            await _dialogService.ShowAsync<PickDialog>("User Picks", parameters, new DialogOptions {
                CloseOnEscapeKey = true,
                NoHeader = true,
                CloseButton = false
            });
        }
    }
    protected async Task RunTimer() {
        _scores = await _espn!.GetScores();
        await InvokeAsync(StateHasChanged);
    }
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            _timer?.Dispose();
        }
    }

    private string DisplayDetails(Competition? competition) {
        if (competition.Status.Type.Name == TypeName.StatusFinal) {
            return "FINAL";
        }
        else if (competition.Status.Type.Name == TypeName.StatusScheduled) {
            //return competition.Date.ToLocalTime().ToString("ddd h:mm");
            return TimeZoneHelpers.ConvertTimeToCST(competition.Date.DateTime).ToString("ddd h:mm");
        }
        else if (competition.Status.Type.Name == TypeName.StatusInProgress) {
            return $"Q{competition.Status.Period} {competition.Status.DisplayClock}";
        }
        return null;
    }
    private double? GetSpread(string teamAbbr) {
        if (_leagueId != 0) {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            var leagueSpread = _db.LeagueJuiceMapping.FirstOrDefault(x => x.Id == _leagueId && x.Season == _scores!.Season.Year);
            if (leagueSpread is not null) {
                if (spread is not null)
                    return spread.HomeTeamSpread + leagueSpread.Juice;
                spread = _odds.FirstOrDefault(x => x.AwayTeam == teamAbbr);
                if (spread is not null)
                    return spread.AwayTeamSpread + leagueSpread.Juice;
            }
        }
        return null;
    }
}
