using System.Data.Entity;
using fourplay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
        _users = _db.Users.ToList();
        _userMapping = _db.LeagueUserMapping.ToList();
        _leagueInfo = _db.LeagueInfo.ToList();
    }
    public async Task AddMapping()
    {
        var parameters = new DialogParameters<ManageUserDialog> { { x => x.Leagues, _leagueInfo.Select(x => x.LeagueName).ToList() }, { x => x.Users, _users.Where(x => x.Email is not null).Select(x => x.Email).ToList()! } };

        var dialog = await DialogService.ShowAsync<ManageUserDialog>("Add Mapping", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var addedUser = (Tuple<string, string>)result.Data;
            if (_userMapping.Where(x => x.User.Email == addedUser.Item1 && x.League.LeagueName == addedUser.Item2).Any())
            {
                Snackbar.Add("User Already Exists", Severity.Error);
            }
            else
            {
                await _db.LeagueUserMapping.AddAsync(new LeagueUserMapping() { LeagueId = _leagueInfo.First(x => x.LeagueName == addedUser.Item2).Id, UserId = _users.First(x => x.Email == addedUser.Item1).Id});
                Snackbar.Add("User Added", Severity.Success);
            }
        }
    }
}
