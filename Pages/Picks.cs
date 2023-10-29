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

public partial class Picks : ComponentBase {
    [Inject]
    private IESPNApiService? _espn { get; set; } = default!;
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    private ESPNScores? _scores = null;
    private List<NFLSpreads>? _odds = null;
    private List<string> _picks = new();
    protected override async Task OnInitializedAsync() {
        _scores = await _espn.GetScores();
        _odds = _db.NFLSpreads.Where(x => x.Season == _scores!.Season.Year && x.NFLWeek == _scores.Week.Number).ToList();

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
    private async Task UnSelectPick(string teamAbbreviation) =>
      _picks.Remove(teamAbbreviation);

    private async Task SelectPick(string teamAbbreviation) =>
        _picks.Add(teamAbbreviation);

    private bool IsSelected(string teamAbbreviation) => _picks.Contains(teamAbbreviation);
    private bool IsDisabled() => _picks.Count == 4;

}
