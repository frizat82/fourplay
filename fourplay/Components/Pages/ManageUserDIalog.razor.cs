using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;
using fourplay.Data;

namespace fourplay.Components.Pages;
[Authorize]
public partial class ManageUserDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public List<ApplicationUser> Users { get; set; } = default!;
    [Parameter] public List<LeagueInfo> Leagues { get; set; } = default!;
    private ApplicationUser? _user;
    private LeagueInfo? _league;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_league is not null && _user is not null);

    private void AddMapping() {
        MudDialog.Close(DialogResult.Ok(new MapUserModel() { User = _user!, League = _league! }));
    }
}