using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;

namespace fourplay.Components.Pages;
[Authorize]
public partial class AddUserDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private string _user;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_user is not null);

    private void AddMapping()
    {
        MudDialog.Close(DialogResult.Ok(new MapUserModel() { Email = _user }));
    }
}