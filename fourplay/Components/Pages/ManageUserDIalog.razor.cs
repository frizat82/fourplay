using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;

namespace fourplay.Components.Pages;
[Authorize]
public partial class ManageUserDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public List<string> Users { get; set; } = default!;
    [Parameter] public List<string> Leagues { get; set; } = default!;
    private string? _user;
    private string? _league;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_league is not null && _user is not null);

    private void AddMapping()
    {
        MudDialog.Close(DialogResult.Ok(new MapUserModel() { Email = _user!, LeagueName = _league! }));
    }
}