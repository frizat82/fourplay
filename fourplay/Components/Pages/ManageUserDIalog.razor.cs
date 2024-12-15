using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;

namespace fourplay.Components.Pages;
[Authorize]
public partial class ManageUserDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public List<string> Users { get; set; }
    [Parameter] public List<string> Leagues { get; set; }
    private string _user;
    private string _league;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_league is not null && _user is not null);

    private void AddMapping()
    {
        MudDialog.Close(DialogResult.Ok(new {_user, _league}));
    }
}