using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;

namespace fourplay.Components.Pages;
[Authorize]
public partial class ManageLeagueDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    private string? _leagueName;
    private int? _juice;
    private int? _season;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_leagueName is not null && _juice is not null && _season is not null);

    private void AddMapping()
    {
        MudDialog.Close(DialogResult.Ok(new CreateLeagueModel(){ LeagueName = _leagueName, Juice = _juice.Value, Season = _season.Value }));
    }
}