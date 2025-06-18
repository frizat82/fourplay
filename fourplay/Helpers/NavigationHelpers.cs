using Microsoft.AspNetCore.Components;
namespace fourplay.Helpers;

public static class NavigationHelpers {
    public static async Task HandleFirstRenderAsync(int leagueId, NavigationManager navigation, Action setInitialized) {
        if (leagueId == 0) {
            navigation.NavigateTo("/leagues");
        }
        else {
            setInitialized();
            // Add any additional logic that needs to be executed after initialization
            await Task.CompletedTask;
        }
    }
}