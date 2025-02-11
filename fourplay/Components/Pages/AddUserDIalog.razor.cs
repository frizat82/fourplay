using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;
using fourplay.Data;

namespace fourplay.Components.Pages;
[Authorize]
public partial class AddUserDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    private string? _user = null;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_user is not null);

    private void AddMapping() {
        MudDialog.Close(DialogResult.Ok(new MapUserModel() { User = new ApplicationUser() { Email = _user! } }));
    }
}