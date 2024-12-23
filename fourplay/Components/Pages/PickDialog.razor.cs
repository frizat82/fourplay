using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;

namespace fourplay.Components.Pages;
[Authorize]
public partial class PickDialog
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public List<string> UserNames { get; set; } = default!;
    [Parameter] public string TeamAbbr { get; set; } = default!;
    [Parameter] public string Logo { get; set; } = default!;
}