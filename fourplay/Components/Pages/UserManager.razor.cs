
using fourplay.Data;
using fourplay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Quartz;
using Serilog;
using System;

namespace fourplay.Components.Pages;
[Authorize(Roles = "Administrator")]
public partial class UserManager : ComponentBase, IDisposable {
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private IDbContextFactory<ApplicationDbContext> _dbContextFactory { get; set; } = default!;
    private ApplicationDbContext _db { get; set; } = default!;
    private List<ApplicationUser> _users { get; set; } = new();
    private List<LeagueUserMapping> _userMapping { get; set; } = new();
    private List<LeagueInfo> _leagueInfo { get; set; } = new();
    private List<LeagueJuiceMapping> _leagueJuiceMapping { get; set; } = new();
    private List<NFLScores> _scores { get; set; } = new();
    private List<NFLSpreads> _spreads { get; set; } = new();
    private IEnumerable<LeagueUsers> _leagueUsers { get; set; } = new List<LeagueUsers>();
    [Inject] private ISchedulerFactory _schedulerFactory { get; set; } = default!;
    private bool _loadingUsers = true;
    private bool _loadingMappings = true;
    private bool _loadingJuice = true;
    private bool _loadingScores = true;

    protected override async Task OnInitializedAsync() {
        _db = _dbContextFactory.CreateDbContext();
        _users = await _db!.Users.ToListAsync();
        //Log.Information("{@Users}", _users);
        _leagueUsers = await _db!.LeagueUsers.ToListAsync();
        _loadingUsers = false;
        _userMapping = await _db.LeagueUserMapping.ToListAsync();
        _loadingMappings = false;
        _leagueInfo = await _db.LeagueInfo.ToListAsync();
        _leagueJuiceMapping = await _db.LeagueJuiceMapping.ToListAsync();
        _loadingJuice = false;
        _scores = await _db.NFLScores.ToListAsync();
        _spreads = await _db.NFLSpreads.ToListAsync();
        _loadingScores = false;
    }
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            _db?.Dispose();
        }
    }

    private async Task ClearScores() {
        int records = 0;
        foreach (var score in _db.NFLScores) {
            _db.NFLScores.Remove(score);
            ++records;
        }
        await _db.SaveChangesAsync();
        Snackbar.Add($"Removed {records} NFLScores", Severity.Success);
    }
    public async Task AddUser() {
        var parameters = new DialogParameters<AddUserDialog>();

        var dialog = await DialogService.ShowAsync<AddUserDialog>("Add User", parameters);
        var result = await dialog.Result;
        if (result is null)
            return;

        if (!result.Canceled) {
            var addedUser = (MapUserModel)result.Data;
            if (_leagueUsers.Where(x => x.GoogleEmail == addedUser.User.Email).Any()) {
                Snackbar.Add("User Already Exists", Severity.Error);
            }
            else {
                await _db.LeagueUsers.AddAsync(new LeagueUsers() { GoogleEmail = addedUser.User.Email });
                await _db.SaveChangesAsync();
                Snackbar.Add("User Added", Severity.Success);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
    public async Task AddMapping() {
        var parameters = new DialogParameters<ManageUserDialog> { { x => x.Leagues, _leagueInfo }, { x => x.Users, _users } };

        var dialog = await DialogService.ShowAsync<ManageUserDialog>("Add Mapping", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            var addedUser = (MapUserModel)result.Data!;
            if (_userMapping.Where(x => x.UserId == addedUser.User.Id && x.LeagueId == addedUser.League.Id).Any()) {
                Snackbar.Add("User Already Exists In League", Severity.Error);
            }
            else {
                await _db.LeagueUserMapping.AddAsync(new LeagueUserMapping() { LeagueId = addedUser!.League.Id, UserId = addedUser.User.Id });
                await _db.SaveChangesAsync();
                Snackbar.Add("User Added To League", Severity.Success);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
    public async Task AddLeague() {
        var parameters = new DialogParameters<ManageLeagueDialog> { };

        var dialog = await DialogService.ShowAsync<ManageLeagueDialog>("Add League", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            var addedLeague = (CreateLeagueModel)result.Data;
            if (_leagueJuiceMapping.Where(x => x.League.LeagueName == addedLeague.LeagueName && x.Season == addedLeague.Season).Any()) {
                Snackbar.Add("League Season Already Exists", Severity.Error);
            }
            else {
                if (!_leagueInfo.Any(x => x.LeagueName == addedLeague.LeagueName)) {
                    Log.Information("Creating {LeagueName}", addedLeague.LeagueName);
                    await _db.LeagueInfo.AddAsync(new LeagueInfo() { LeagueName = addedLeague.LeagueName });
                    await _db.SaveChangesAsync();
                }
                Log.Information("Adding {Season} {Juice} {LeagueName}", addedLeague.Season, addedLeague.Juice, addedLeague.LeagueName);
                var createdLeague = _db.LeagueInfo.Where(x => x.LeagueName == addedLeague.LeagueName).First();
                await _db.LeagueJuiceMapping.AddAsync(new LeagueJuiceMapping() {
                    Juice = addedLeague.Juice,
                    JuiceConference = addedLeague.JuiceConference, JuiceDivisonal = addedLeague.JuiceDivisional,
                    League = createdLeague, Season = addedLeague.Season
                });
                await _db.SaveChangesAsync();
                Snackbar.Add("League Season Added", Severity.Success);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
    public async Task RunSpreads() {
        // Create a scheduler
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey("NFL Spreads"));
        Log.Information("Started Spread Job");
        Snackbar.Add("Started Spread Job", Severity.Success);
    }
    public async Task RunScores() {
        // Create a scheduler
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey("NFL Scores"));
        Log.Information("Started Scores Job");
        Snackbar.Add("Started Scores Job", Severity.Success);
    }
    public async Task RunUserJob() {
        // Create a scheduler
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey("User Manager"));
        Log.Information("Started User Manager Job");
        Snackbar.Add("Started User Manager Job", Severity.Success);
    }
}
