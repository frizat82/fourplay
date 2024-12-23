using fourplay.Data;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Serilog;

namespace fourplay.Components.Pages;
[Authorize]
public partial class Picks : ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Inject] private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; } = default!;
    [Inject] private ILoginHelper _loginHelper { get; set; } = default!;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    private ESPNScores? _scores = null;
    private List<NFLSpreads>? _odds = null;
    private List<string> _picks = new();
    private int? _leagueId { get; set; }
    private bool _locked = false;

    protected override async Task OnInitializedAsync()
    {
        using var db = _dbContextFactory.CreateDbContext();
        _scores = await _espn.GetScores();
        _odds = db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null)
        {
            var picks = db.NFLPicks.Where(x => x.UserId == usrId.Id && x.Season == _scores!.Season.Year
            && x.NFLWeek == _scores.Week.Number);
            if (picks.Any())
            {
                _picks = await picks.Select(x => x.Team).ToListAsync();
                _locked = true;
            }
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
            if (leagueId == 0)
                Navigation.NavigateTo("/leagues");
            _leagueId = leagueId;
            await OnInitializedAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
    private async Task SubmitPicks()
    {
        Log.Information("Submitting Picks");
        using var db = _dbContextFactory.CreateDbContext();
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null)
        {
            await db.NFLPicks.AddRangeAsync(_picks.Select(x => new NFLPicks()
            {
                LeagueId = _leagueId.Value,
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

    private double? GetSpread(string teamAbbr)
    {
        //TODO: Add Caching
        using var db = _dbContextFactory.CreateDbContext();
        if (_leagueId != 0)
        {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            var leagueSpread = db.LeagueJuiceMapping.FirstOrDefault(x => x.Id == _leagueId && x.Season == _scores!.Season.Year);
            if (leagueSpread is not null)
            {
                if (spread is not null)
                    return spread.HomeTeamSpread + leagueSpread.Juice;
                spread = _odds.FirstOrDefault(x => x.AwayTeam == teamAbbr);
                if (spread is not null)
                    return spread.AwayTeamSpread + leagueSpread.Juice;
            }
        }
        return null;

    }
    private void UnSelectPick(string teamAbbreviation)
    {
        if (!IsPicksLocked())
        {
            _picks.Remove(teamAbbreviation);
        }
    }

    private void SelectPick(string teamAbbreviation)
    {
        if (!IsPicksLocked())
        {
            _picks.Add(teamAbbreviation);
        }
    }
    private string? DisplayDetails(Competition? competition)
    {
        if (competition.Status.Type.Name == TypeName.StatusScheduled)
        {
            return competition.Date.ToLocalTime().ToString("ddd, MMMM dd HH:mm");
        }
        return "          ";
    }
    private bool IsSelected(string teamAbbreviation) => _picks.Contains(teamAbbreviation);
    private bool IsDisabled() => _picks.Count == 4;
    private bool IsGameStarted(Competition competition) => competition.Status.Type.Name != TypeName.StatusScheduled;
    private bool IsGameStartedOrDisabled(Competition competition) => IsGameStarted(competition) || IsDisabled();
    private bool IsPicksLocked() => _locked;

}
