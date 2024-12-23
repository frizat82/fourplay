using System;
using System.Data;
using Blazored.LocalStorage;
using fourplay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace fourplay.Components.Pages;
[Authorize]
public partial class Leaderboard : ComponentBase
{
    [Inject] private ApplicationDbContext? _db { get; set; } = default!;
    [Inject] ILocalStorageService _localStorage { get; set; } = default!;
    [Inject] private IESPNApiService? _espn { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private int _leagueId;
    private DataTable _dataTable = new();
    private bool _loading = true;
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        Log.Information("Loading Scoreboard {LeagueId}", _leagueId);
        try
        {
            var scores = await _espn!.GetScores();
            if (scores is null)
                return;

            var allUsers = await _db!.LeagueUserMapping.ToListAsync();
            Log.Information("{@AllUsers}", allUsers);

            var leagueUsers = await _db!.LeagueUserMapping
                .Where(lum => lum.LeagueId == _leagueId)
                .Select(lum => lum.UserId)
                .ToListAsync();

            Log.Information("{@LeagueUsers}", leagueUsers);
            var userScores = await _db.NFLScores
                .Where(score => score.Season == scores.Season.Year)
                .ToListAsync();

            var spreads = await _db.NFLSpreads.Where(spread => spread.Season == scores.Season.Year).ToListAsync();

            //Log.Information("{@Spreads}", spreads);
            var leagueInfo = await _db.LeagueJuiceMapping
                .Where(li => li.LeagueId == _leagueId)
                .FirstOrDefaultAsync();

            Log.Information("{@LeagueInfo}", leagueInfo);
            _dataTable = new DataTable();
            _dataTable.Columns.Add("User");

            for (int week = 1; week <= 16; week++)
            {
                _dataTable.Columns.Add($"Week {week}");
            }

            foreach (var userId in leagueUsers)
            {
                DataRow row = _dataTable.NewRow();
                row["User"] = userId;

                for (int week = 1; week <= 16; week++)
                {
                    var userPicks = await _db.NFLPicks
                        .Where(pick => pick.UserId == userId && pick.Season == scores.Season.Year && pick.NFLWeek == week)
                        .ToListAsync();

                    bool allPicksBeatSpread = userPicks.All(pick =>
                {
                    var score = userScores.FirstOrDefault(s => s.NFLWeek == week);
                    var homeTeamSpread = spreads.FirstOrDefault(s => s.NFLWeek == week && s.HomeTeam == pick.Team);
                    var awayTeamSpread = spreads.FirstOrDefault(s => s.NFLWeek == week && s.AwayTeam == pick.Team);
                    if (score == null || homeTeamSpread == null || awayTeamSpread == null) return false;
                    if (homeTeamSpread is not null)
                    {
                        var adjustedSpread = homeTeamSpread.HomeTeamSpread + leagueInfo!.Juice;
                        return (score.HomeTeamScore - score.AwayTeamScore) > adjustedSpread;
                    }
                    else
                    {
                        var adjustedSpread = awayTeamSpread.AwayTeamSpread + leagueInfo!.Juice;
                        return (score.AwayTeamScore - score.HomeTeamScore) > adjustedSpread;
                    }
                });

                    row[$"Week {week}"] = allPicksBeatSpread ? "Yes" : "No";
                }
                Log.Information("{@Row}", row);
                _dataTable.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading leaderboard");
        }
        _loading = false;

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
}
