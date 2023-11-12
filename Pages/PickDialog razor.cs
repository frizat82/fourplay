using fourplay.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace fourplay.Pages;
public partial class PickDialog : ComponentBase {
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    [Parameter] public List<string> UserNames { get; set; }
}