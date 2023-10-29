using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fourplay.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NodaTime;
using System.Data.Entity;

namespace fourplay.Pages;

public partial class Scores : ComponentBase {
    [Inject]
    private IESPNApiService? _espn { get; set; } = default!;
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    private ESPNScores? _scores = null;
    private System.Timers.Timer _timer = new System.Timers.Timer();
    private List<NFLSpreads>? _odds = null;
    protected override async Task OnInitializedAsync() {
        _scores = await _espn.GetScores();
        _odds = _db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();
        RunTimer();
        _timer.Elapsed += TimeElapsed;
        _timer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        _timer.AutoReset = true;
        _timer.Enabled = true;

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
        var spread = _odds.FirstOrDefault(x => x.HomeTeam == teamAbbr);
        if (spread is not null)
            return (spread.FourPlayHomeSpread == 0 ? spread.HomeTeamSpread : spread.FourPlayHomeSpread);
        spread = _odds.FirstOrDefault(x => x.AwayTeam == teamAbbr);
        if (spread is not null)
            return (spread.FourPlayAwaySpread == 0 ? spread.AwayTeamSpread : spread.FourPlayAwaySpread);
        return null;
    }
}
