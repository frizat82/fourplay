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

    [Inject] private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; } = default!;
    [Inject] private ILoginHelper _loginHelper { get; set; } = default!;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    internal ESPNScores? _scores = null;
    internal List<NFLSpreads>? _odds = null;
    internal List<string> _picks = new();
    internal Dictionary<Competition, PickType> _picksOverUnder = new();
    private int _leagueId = 0;
    private bool _loading = true;
    private bool _locked = false;
    private bool _isPostseason = false;
    private long _week = 0;

    protected override async Task OnInitializedAsync() {
        _loading = true;
        using var db = _dbContextFactory.CreateDbContext();
        _scores = await _espn!.GetScores();
        if (_scores is null) return;
        if (_scores!.Season.Type == (int)TypeOfSeason.PostSeason)
            _isPostseason = true;
        _week = _scores!.Week!.Number;
        _odds = db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null) {
            var picks = db.NFLPicks.Where(x => x.UserId == usrId.Id && x.Season == _scores!.Season.Year
            && x.NFLWeek == _scores.Week.Number);
            if (picks.Any()) {
                _picks = await picks.Select(x => x.Team).ToListAsync();
                _locked = true;
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
    private async Task SubmitPicks() {
        Log.Information("Submitting Picks");
        using var db = _dbContextFactory.CreateDbContext();
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null) {
            await db.NFLPicks.AddRangeAsync(_picks.Select(x => new NFLPicks() {
                LeagueId = _leagueId,
                NFLWeek = (int)_scores.Week.Number,
                Season = (int)_scores!.Season.Year,
                Team = x,
                UserId = usrId.Id
            }));
            await db.SaveChangesAsync();
            Snackbar.Add("Picks Added", Severity.Success);
            _locked = true;
            await InvokeAsync(StateHasChanged);
        }
    }
    private double? GetOverUnder(string teamAbbr) {
        //TODO: Add Caching
        using var db = _dbContextFactory.CreateDbContext();
        if (_leagueId != 0) {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            if (spread is not null)
                return spread.OverUnder;
            spread = _odds.FirstOrDefault(x => x.AwayTeam == teamAbbr);
            if (spread is not null)
                return spread.OverUnder;
        }
        return null;
    }
    private double? GetSpread(string teamAbbr) {
        //TODO: Add Caching
        using var db = _dbContextFactory.CreateDbContext();
        if (_leagueId != 0) {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            var leagueSpread = db.LeagueJuiceMapping.FirstOrDefault(x => x.Id == _leagueId && x.Season == _scores!.Season.Year);
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
    private void UnSelectOverUnderPick(Competition competition) {
        if (!IsPicksLocked()) {
            _picksOverUnder.Remove(competition);
        }
    }

    private void SelectOverUnderPick(Competition competition, PickType pickType) {
        if (!IsPicksLocked()) {
            _picksOverUnder.Add(competition, pickType);
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
    private string? DisplayDetails(Competition? competition) {
        if (competition.Status.Type.Name == TypeName.StatusScheduled) {
            return competition.Date.ToLocalTime().ToString("ddd, MMMM dd HH:mm");
        }
        else if (competition.Status.Type.Name == TypeName.StatusInProgress) {
            return "In Progress";
        }
        else if (competition.Status.Type.Name == TypeName.StatusFinal) {
            return "Final";
        }
        return "          ";
    }
    public bool IsGameStartedOrDisabledPicks(Competition competition) => GameHelpers.IsGameStarted(competition) || IsDisabled();
    private bool IsSelected(string teamAbbreviation) => _picks.Contains(teamAbbreviation);
    private bool IsOverUnderSelected(Competition competition) => _picksOverUnder.ContainsKey(competition);
    private bool IsDisabled() => _picks.Count == 4 || _picks.Count + _picksOverUnder.Count == 4;
    private bool IsPicksLocked() => _locked;

}
