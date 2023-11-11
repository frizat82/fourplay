using fourplay.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NodaTime;

namespace fourplay.Pages;

public partial class Scores : ComponentBase {
    [Inject]
    private IESPNApiService? _espn { get; set; } = default!;
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    private ESPNScores? _scores = null;
    private System.Timers.Timer _timer = new System.Timers.Timer();
    private List<NFLSpreads>? _odds = null;
    [Inject] Blazored.LocalStorage.ILocalStorageService _localStorage { get; set; }
    private int _leagueId = 0;
    protected override async Task OnInitializedAsync() {
        _scores = await _espn.GetScores();
        _odds = _db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();
        RunTimer();
        _timer.Elapsed += TimeElapsed;
        _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            var leagueId = await _localStorage.GetItemAsync<int>("leagueId");
            if (leagueId > 0)
                _leagueId = leagueId;
            await InvokeAsync(StateHasChanged);
        }
    }
    public string GetIcon(Competitor baseTeam, Competitor compareTeam) {
        var spread = GetSpread(baseTeam.Team.Abbreviation);
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Icons.Material.Filled.GppGood;
        else
            return Icons.Material.Filled.GppBad;
    }
    public Color GetColor(Competitor baseTeam, Competitor compareTeam) {
        var spread = GetSpread(baseTeam.Team.Abbreviation);
        if ((baseTeam.Score + spread - compareTeam.Score) > 0)
            return Color.Success;
        else
            return Color.Error;
    }
    public async Task<int> GetUserPicks(string teamAbbr) {
        var picks = _db.NFLPicks.Where(x => x.LeagueId == _leagueId && x.Season == _scores!.Season.Year
        && x.NFLWeek == _scores.Week.Number && x.Team == teamAbbr);
        return picks.Count();
    }
    private async void TimeElapsed(object? sender, System.Timers.ElapsedEventArgs e) {
        await InvokeAsync(StateHasChanged);
    }

    protected async void RunTimer() {
        _scores = await _espn.GetScores();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            _timer.Dispose();
        }
    }

    private DateTime ConvertTimeToCST(DateTime utcDateTime) {
        // Create an instance of the BclDateTimeZone representing the Central Standard Time (CST) zone.
        var cstZone = DateTimeZoneProviders.Tzdb["America/Chicago"];

        // Change the Kind to Local.
        var utc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        // Create an Instant from the UTC DateTime.
        var instant = Instant.FromDateTimeUtc(utc);

        // Convert the instant to the Central Standard Time.
        var zonedDateTime = instant.InZone(cstZone);

        // Return the DateTime in CST.
        return zonedDateTime.ToDateTimeUnspecified();
    }
    private string DisplayDetails(Competition? competition) {
        if (competition.Status.Type.Name == TypeName.StatusFinal) {
            return "FINAL";
        }
        else if (competition.Status.Type.Name == TypeName.StatusScheduled) {
            return competition.Date.ToLocalTime().ToString("ddd h:mm");
        }
        else if (competition.Status.Type.Name == TypeName.StatusInProgress) {
            return $"Q{competition.Status.Period} {competition.Status.DisplayClock}";
        }
        return null;
    }
    private double? GetSpread(string teamAbbr) {
        if (_leagueId != 0) {
            var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
            var leagueSpread = _db.LeagueInfo.FirstOrDefault(x => x.Id == _leagueId && x.Season == _scores!.Season.Year);
            if (leagueSpread is not null) {
                if (spread is not null)
                    return spread.FourPlayHomeSpread == 0 ? spread.HomeTeamSpread + leagueSpread.Juice : spread.FourPlayHomeSpread;
                spread = _odds.FirstOrDefault(x => x.AwayTeam == teamAbbr);
                if (spread is not null)
                    return spread.FourPlayAwaySpread == 0 ? spread.AwayTeamSpread + leagueSpread.Juice : spread.FourPlayAwaySpread;
            }
        }
        return null;

    }
}
