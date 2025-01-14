using fourplay.Data;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using fourplay.Helpers;
using Serilog;
using System.Runtime.CompilerServices;
using fourplay.Services.Interfaces;
using fourplay.Models.Enum;
[assembly: InternalsVisibleTo("unitTests")]
namespace fourplay.Components.Pages;
[Authorize]
public partial class Picks : ComponentBase {
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ISpreadCalculator SpreadCalculator { get; set; } = default!;

    [Inject] private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; } = default!;
    [Inject] private ILoginHelper _loginHelper { get; set; } = default!;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    internal ESPNScores? _scores = null;
    internal HashSet<string> _picks = new();
    internal HashSet<NFLPostSeasonPicks> _picksOverUnder = new();
    private int _leagueId = 0;
    private bool _loading = true;
    private bool _locked = false;
    private bool _isPostSeason = false;
    private long _week = 0;
    private ApplicationUser _user;

    protected override async Task OnInitializedAsync() {
        _loading = true;
        using var db = _dbContextFactory.CreateDbContext();
        _scores = await _espn!.GetScores();
        while (_scores is null) {
            _scores = await _espn.GetScores();
        }
        _isPostSeason = _scores!.IsPostSeason();
        _week = _scores!.Week!.Number;
        _user = await _loginHelper!.GetUserDetails()!;
        if (_user is not null) {
            var picks = db.NFLPicks.Where(x => x.UserId == _user.Id && x.Season == _scores!.Season.Year
            && x.NFLWeek == _scores.Week.Number);
            if (picks.Any()) {
                _picks = picks.Select(x => x.Team).ToHashSet();
                _locked = true;
            }
            if (_isPostSeason) {
                var postSeasonPicks = db.NFLPostSeasonPicks.Where(x => x.UserId == _user.Id && x.Season == _scores!.Season.Year
                && x.NFLWeek == _scores.Week.Number);
                if (postSeasonPicks.Any()) {
                    _picksOverUnder = postSeasonPicks.ToHashSet();
                    _locked = true;
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
                SpreadCalculator.Configure(_leagueId, (int)_week, (int)_scores!.Season.Year, _isPostSeason);
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
    private async Task SubmitPicks() {
        Log.Information("Submitting Picks");
        using var db = _dbContextFactory.CreateDbContext();
        if (_user is not null) {
            if (!_isPostSeason) {
                await db.NFLPicks.AddRangeAsync(_picks.Select(x => new NFLPicks() {
                    LeagueId = _leagueId,
                    NFLWeek = (int)_scores!.Week.Number,
                    Season = (int)_scores!.Season.Year,
                    Team = x,
                    UserId = _user.Id
                }));
            }
            else {
                await db.NFLPostSeasonPicks.AddRangeAsync(_picksOverUnder);
                await db.NFLPostSeasonPicks.AddRangeAsync(_picks.Select(x => new NFLPostSeasonPicks() {
                    LeagueId = _leagueId,
                    NFLWeek = (int)_scores!.Week.Number,
                    Season = (int)_scores!.Season.Year,
                    Team = x,
                    Pick = PickType.Spread,
                    UserId = _user.Id
                }));
            }
            await db.SaveChangesAsync();
            Snackbar.Add("Picks Added", Severity.Success);
            _locked = true;
            await InvokeAsync(StateHasChanged);
        }
    }
    private NFLPostSeasonPicks CompetitionToPostSeasonPick(string teamAbbr, PickType pickType) {
        return new NFLPostSeasonPicks() {
            LeagueId = _leagueId,
            NFLWeek = (int)_scores!.Week.Number,
            Season = (int)_scores!.Season.Year,
            Team = teamAbbr,
            Pick = pickType,
            UserId = _user.Id
        };
    }
    private void UnSelectOverUnderPick(string teamAbbr, PickType pickType) {
        if (!IsPicksLocked()) {
            var pick = CompetitionToPostSeasonPick(teamAbbr, pickType);
            _picksOverUnder.Remove(pick);
        }
    }
    private void SelectOverUnderPick(string teamAbbr, PickType pickType) {
        if (!IsPicksLocked()) {
            var pick = CompetitionToPostSeasonPick(teamAbbr, pickType);
            _picksOverUnder.Add(pick);
        }
    }
    private void UnSelectPick(string teamAbbreviation) {
        if (!IsPicksLocked()) {
            _picks.Remove(teamAbbreviation);
        }
    }

    private void SelectPick(string teamAbbreviation) {
        if (!IsPicksLocked()) {
            _picks.Add(teamAbbreviation);
        }
    }
    private string? DisplayDetails(Competition competition) {
        if (competition.Status.Type.Name == TypeName.StatusScheduled) {
            return TimeZoneHelpers.ConvertTimeToCST(competition.Date.DateTime).ToString("ddd, MMMM dd hh:mm tt");
        }
        else if (competition.Status.Type.Name == TypeName.StatusInProgress) {
            return "In Progress";
        }
        else if (competition.Status.Type.Name == TypeName.StatusFinal) {
            return "Final";
        }
        return "          ";
    }
    //public bool IsGameStartedOrDisabledPicks(Competition competition, PickType pickType) => GameHelpers.IsGameStarted(competition) || IsDisabled() || IsOverUnderSelected(competition, pickType);
    public bool IsGameStartedOrDisabledPicks(Competition competition) => GameHelpers.IsGameStarted(competition) || IsDisabled();
    private bool IsSelected(string teamAbbreviation) => _picks.Contains(teamAbbreviation);
    private bool IsOverUnderSelected(string teamAbbr, PickType pickType) {
        var pick = CompetitionToPostSeasonPick(teamAbbr, pickType);
        return _picksOverUnder.Contains(pick);
    }
    private bool IsDisabled() => _picks.Count == @GameHelpers.GetRequiredPicks(_week, _isPostSeason) || _picks.Count + _picksOverUnder.Count == @GameHelpers.GetRequiredPicks(_week, _isPostSeason);
    private bool IsPicksLocked() => _locked;

}
