using System.Data.Entity;
using fourplay.Data;
using fourplay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Quartz;
using Serilog;

namespace fourplay.Components.Pages;
[Authorize(Roles = "Administrator")]
public partial class UserManager : ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private ApplicationDbContext? _db { get; set; } = default!;
    private List<ApplicationUser> _users { get; set; } = new();
    private List<LeagueUserMapping> _userMapping { get; set; } = new();
    private List<LeagueInfo> _leagueInfo { get; set; } = new();
    private List<LeagueJuiceMapping> _leagueJuiceMapping { get; set; } = new();
    [Inject] private ISchedulerFactory _schedulerFactory { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
        _users = _db.Users.ToList();
        _userMapping = _db.LeagueUserMapping.ToList();
        _leagueInfo = _db.LeagueInfo.ToList();
        _leagueJuiceMapping = _db.LeagueJuiceMapping.ToList();
    }
    public async Task AddMapping()
    {
        var parameters = new DialogParameters<ManageUserDialog> { { x => x.Leagues, _leagueInfo.Select(x => x.LeagueName).ToList() }, { x => x.Users, _users.Where(x => x.Email is not null).Select(x => x.Email).ToList()! } };

        var dialog = await DialogService.ShowAsync<ManageUserDialog>("Add Mapping", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var addedUser = (MapUserModel)result.Data;
            if (_userMapping.Where(x => x.User.Email == addedUser.Email && x.League.LeagueName == addedUser.LeagueName).Any())
            {
                Snackbar.Add("User Already Exists", Severity.Error);
            }
            else
            {
                await _db.LeagueUserMapping.AddAsync(new LeagueUserMapping() { LeagueId = _leagueInfo.First(x => x.LeagueName == addedUser.LeagueName).Id, UserId = _users.First(x => x.Email == addedUser.Email).Id });
                await _db.SaveChangesAsync();
                Snackbar.Add("User Added", Severity.Success);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
    public async Task AddLeague()
    {
        var parameters = new DialogParameters<ManageLeagueDialog> { };

        var dialog = await DialogService.ShowAsync<ManageLeagueDialog>("Add League", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var addedLeague = (CreateLeagueModel)result.Data;
            if (_leagueJuiceMapping.Where(x => x.League.LeagueName == addedLeague.LeagueName && x.Season == addedLeague.Season).Any())
            {
                Snackbar.Add("League Season Already Exists", Severity.Error);
            }
            else
            {
                if (!_leagueInfo.Any(x => x.LeagueName == addedLeague.LeagueName))
                {
                    Log.Information("Creating {LeagueName}", addedLeague.LeagueName);
                    await _db.LeagueInfo.AddAsync(new LeagueInfo() { LeagueName = addedLeague.LeagueName });
                    await _db.SaveChangesAsync();
                }
                Log.Information("Adding {Season} {Juice} {LeagueName}", addedLeague.Season, addedLeague.Juice, addedLeague.LeagueName);
                var createdLeague = _db.LeagueInfo.Where(x => x.LeagueName == addedLeague.LeagueName).First();
                await _db.LeagueJuiceMapping.AddAsync(new LeagueJuiceMapping() { Juice = addedLeague.Juice, League = createdLeague, Season = addedLeague.Season });
                await _db.SaveChangesAsync();
                Snackbar.Add("League Season Added", Severity.Success);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
    public async Task RunSpreads()
    {
        // Create a scheduler
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.TriggerJob(new JobKey("NFL Spreads"));
        Log.Information("Started Spread Job");
        Snackbar.Add("Started Spread Job", Severity.Success);
    }
}
