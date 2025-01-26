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
using fourplay.Models.Enum;

namespace fourplay.Components.Pages;
[Authorize]
public partial class Scores : ComponentBase, IDisposable {
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private ApplicationDbContext? _db { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISpreadCalculator SpreadCalculator { get; set; } = default!;
    private ESPNScores? _scores = null;
    private Dictionary<string, List<string>> _userPicks = new();
    private Dictionary<string, List<string>> _userOverPicks = new();
    private Dictionary<string, List<string>> _userUnderPicks = new();
    private System.Timers.Timer _timer = new();
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    private int _leagueId = 0;
    [Inject] IDialogService _dialogService { get; set; } = default!;
    private bool _loading = true;
    private bool _isPostSeason = false;
    private long _week = 0;
    protected override async Task OnInitializedAsync() {
        _loading = true;
        _scores = await _espn!.GetScores();
        while (_scores is null) {
            _scores = await _espn.GetScores();
        }
        _isPostSeason = _scores!.IsPostSeason();
        _week = _scores!.Week!.Number;
        await RunTimer();
        _timer.Elapsed += TimeElapsed;
        _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        foreach (var scoreEvent in _scores?.Events!) {
            foreach (var competition in scoreEvent.Competitions.OrderBy(x => x.Competitors[1].Team.Abbreviation)) {
                _userPicks.Add(GameHelpers.GetHomeTeamAbbr(competition), new List<string>());
                _userPicks.Add(GameHelpers.GetAwayTeamAbbr(competition), new List<string>());
                _userOverPicks.Add(GameHelpers.GetHomeTeamAbbr(competition), new List<string>());
                _userOverPicks.Add(GameHelpers.GetAwayTeamAbbr(competition), new List<string>());
                _userUnderPicks.Add(GameHelpers.GetHomeTeamAbbr(competition), new List<string>());
                _userUnderPicks.Add(GameHelpers.GetAwayTeamAbbr(competition), new List<string>());
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
                SpreadCalculator.Configure(_leagueId, (int)_week, (int)_scores!.Season.Year, _isPostSeason);
                foreach (var scoreEvent in _scores?.Events!) {
                    foreach (var competition in scoreEvent.Competitions.OrderBy(x => x.Competitors[1].Team.Abbreviation)) {
                        if (GameHelpers.IsGameStarted(competition)) {
                            _userPicks[GameHelpers.GetHomeTeamAbbr(competition)] = await GetUserNamedPicks(GameHelpers.GetHomeTeamAbbr(competition));
                            _userPicks[GameHelpers.GetAwayTeamAbbr(competition)] = await GetUserNamedPicks(GameHelpers.GetAwayTeamAbbr(competition));
                            _userOverPicks[GameHelpers.GetHomeTeamAbbr(competition)] = await GetUserNamedOverUnderPicks(GameHelpers.GetHomeTeamAbbr(competition), PickType.Over);
                            _userOverPicks[GameHelpers.GetAwayTeamAbbr(competition)] = await GetUserNamedOverUnderPicks(GameHelpers.GetAwayTeamAbbr(competition), PickType.Over);
                            _userUnderPicks[GameHelpers.GetHomeTeamAbbr(competition)] = await GetUserNamedOverUnderPicks(GameHelpers.GetHomeTeamAbbr(competition), PickType.Under);
                            _userUnderPicks[GameHelpers.GetAwayTeamAbbr(competition)] = await GetUserNamedOverUnderPicks(GameHelpers.GetAwayTeamAbbr(competition), PickType.Under);
                        }
                    }
                }
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
        var spread = SpreadCalculator.GetSpread(baseTeam.Team.Abbreviation) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Icons.Material.Filled.GppGood;
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Icons.Material.Filled.GppGood;
        else
            return Icons.Material.Filled.GppBad;
    }
    public string GetIcon(Competition competition, PickType pickType) {
        var overUnder = SpreadCalculator.GetOverUnder(GameHelpers.GetAwayTeamAbbr(competition), pickType) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Icons.Material.Filled.GppGood;
        if (pickType == PickType.Over) {
            if (GameHelpers.GetHomeTeamScore(competition) + GameHelpers.GetAwayTeamScore(competition) > overUnder)
                return Icons.Material.Filled.GppGood;
            else
                return Icons.Material.Filled.GppBad;
        }
        else {
            if (GameHelpers.GetHomeTeamScore(competition) + GameHelpers.GetAwayTeamScore(competition) < overUnder)
                return Icons.Material.Filled.GppGood;
            else
                return Icons.Material.Filled.GppBad;
        }
    }
    public Color GetColor(Competition competition, Competitor baseTeam, Competitor compareTeam) {
        var spread = SpreadCalculator.GetSpread(baseTeam.Team.Abbreviation) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Color.Success;
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Color.Success;
        else
            return Color.Error;
    }
    public Color GetColor(Competition competition, PickType pickType) {
        var overUnder = SpreadCalculator.GetOverUnder(GameHelpers.GetAwayTeamAbbr(competition), pickType) ?? 0;
        if (!GameHelpers.IsGameStarted(competition)) return Color.Success;
        if (pickType == PickType.Over) {
            if (GameHelpers.GetHomeTeamScore(competition) + GameHelpers.GetAwayTeamScore(competition) > overUnder)
                return Color.Success;
            else
                return Color.Error;
        }
        else {
            if (GameHelpers.GetHomeTeamScore(competition) + GameHelpers.GetAwayTeamScore(competition) < overUnder)
                return Color.Success;
            else
                return Color.Error;
        }
    }
    public async Task<List<string>> GetUserNamedPicks(string teamAbbr) {
        var picks = await _db!.NFLPicks.Where(x => x.LeagueId == _leagueId && x.Season == _scores!.Season.Year
    && x.NFLWeek == _scores.Week.Number && x.Team == teamAbbr).Select(y => y.User.NormalizedUserName).ToListAsync();
        if (picks is null) {
            return new List<string>();
        }
        return picks!;
    }
    public async Task<List<string>> GetUserNamedOverUnderPicks(string teamAbbr, PickType pickType) {
        var picks = await _db!.NFLPostSeasonPicks.Where(x => x.LeagueId == _leagueId && x.Season == _scores!.Season.Year && x.Pick == pickType
        && x.NFLWeek == _scores.Week.Number && x.Team == teamAbbr).Select(y => y.User.NormalizedUserName).ToListAsync();
        if (picks is null) {
            return new List<string>();
        }
        return picks!;
    }

    public int GetUserPicks(Competition competition, string teamAbbr) {
        if (!GameHelpers.IsGameStarted(competition)) return 0;
        return _userPicks[teamAbbr].Count + _userOverPicks[teamAbbr].Count + _userUnderPicks[teamAbbr].Count;
    }
    private void TimeElapsed(object? sender, System.Timers.ElapsedEventArgs e) => RunTimer();
    private async Task ShowDialog(string teamAbbr, string logo) {
        var usrNames = _userPicks![teamAbbr];
        var usrOverNames = _userOverPicks[teamAbbr];
        var usrUnderNames = _userUnderPicks[teamAbbr];
        if (usrNames.Count + usrOverNames.Count + usrUnderNames.Count > 0) {
            var parameters = new DialogParameters<PickDialog> {
            { x => x.UserNames, usrNames },
               { x => x.UserNamesOver, usrOverNames },
                  { x => x.UserNamesUnder, usrUnderNames },
            { x => x.TeamAbbr, teamAbbr},
            {x => x.Logo, logo}};
            await _dialogService.ShowAsync<PickDialog>("User Picks", parameters, new DialogOptions {
                CloseOnEscapeKey = true,
                NoHeader = false,
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
}
