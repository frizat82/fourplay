using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace fourplay.Components.Layout;

[ExcludeFromCodeCoverage]
public partial class MainLayout : LayoutComponentBase {
    public readonly MudTheme MyTheme = new() { PaletteDark = new PaletteDark { Primary = Colors.Green.Darken2, TableHover = Colors.Green.Darken2 } };

    private bool _drawerOpen = true;

    private void DrawerToggle() {
        _drawerOpen = !_drawerOpen;
    }
}
