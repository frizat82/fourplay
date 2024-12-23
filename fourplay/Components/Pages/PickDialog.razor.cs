using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;

namespace fourplay.Components.Pages;
[Authorize]
public partial class PickDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public List<string> UserNames { get; set; }
    [Parameter] public string TeamAbbr { get; set; }
    [Parameter] public string Logo { get; set; }
}