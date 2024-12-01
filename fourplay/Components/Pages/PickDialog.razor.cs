using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace fourplay.Components.Pages;
public partial class PickDialog {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public List<string> UserNames { get; set; }
    [Parameter] public string TeamAbbr { get; set; }
    [Parameter] public string Logo { get; set; }
}