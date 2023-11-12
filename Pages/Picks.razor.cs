using fourplay.Data;
using Microsoft.AspNetCore.Components;
namespace fourplay.Pages;

public partial class Picks : ComponentBase {
    [Inject]
    private IESPNApiService? _espn { get; set; } = default!;
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    [Inject]
    private ILoginHelper _loginHelper { get; set; } = default!;
    [Inject] Blazored.LocalStorage.ILocalStorageService _localStorage { get; set; }
    private ESPNScores? _scores = null;
    private List<NFLSpreads>? _odds = null;
    private List<string> _picks = new();
    private int? _leagueId { get; set; }
    private bool _locked = false;

    protected override async Task OnInitializedAsync() {
        _scores = await _espn.GetScores();
        _odds = _db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();
        var usrId = await _loginHelper.GetUserDetails();
        var picks = _db.NFLPicks.Where(x => x.UserId == usrId.Id && x.Season == _scores!.Season.Year
        && x.NFLWeek == _scores.Week.Number);
        if (picks.Any()) {
            _picks = picks.Select(x => x.Team).ToList();
            _locked = true;
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
            if (leagueId > 0)
                _leagueId = leagueId;
            await InvokeAsync(StateHasChanged);
        }
    }
    private async Task SubmitPicks() {
        var usrId = await _loginHelper.GetUserDetails();
        if (usrId is not null) {
            await _db.NFLPicks.AddRangeAsync(_picks.Select(x => new NFLPicks() {
                LeagueId = _leagueId.Value, NFLWeek = (int)_scores.Week.Number,
                Season = (int)_scores!.Season.Year,
                Team = x, UserId = usrId.Id
            }));
            await _db.SaveChangesAsync();
            _locked = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private double? GetSpread(string teamAbbr) {
        if (_leagueId != 0) {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            var leagueSpread = _db.LeagueInfo.FirstOrDefault(x => x.Id == _leagueId && x.Season == _scores!.Season.Year);
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

    private bool IsSelected(string teamAbbreviation) => _picks.Contains(teamAbbreviation);
    private bool IsDisabled() => _picks.Count == 4;
    private bool IsGameStarted(Competition competition) => competition.Status.Type.Name != TypeName.StatusScheduled;
    private bool IsGameStartedOrDisabled(Competition competition) => IsGameStarted(competition) || IsDisabled();
    private bool IsPicksLocked() => _locked;

}
