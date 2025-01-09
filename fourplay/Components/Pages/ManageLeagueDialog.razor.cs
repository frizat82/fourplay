using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;
using fourplay.Models;

namespace fourplay.Components.Pages;
[Authorize]
public partial class ManageLeagueDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    private string? _leagueName;
    private int? _juice;
    private int? _juiceDivisional;
    private int? _juiceConference;
    private int? _season;
    private void Cancel() => MudDialog.Cancel();
    public bool IsEnabled() => !(_leagueName is not null && _juice is not null
    && _juiceDivisional is not null && _juiceConference is not null && _season is not null);

    private void AddMapping() {
        MudDialog.Close(DialogResult.Ok(new CreateLeagueModel() {
            LeagueName = _leagueName!, Juice = _juice!.Value,
            JuiceConference = _juiceConference.Value, JuiceDivisional = _juiceDivisional.Value, Season = _season!.Value
        }));
    }
}