using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using fourplay.Data;
using Microsoft.AspNetCore.Components;
using fourplay.Components.Account;
using Serilog;
namespace fourplay.Components.Pages;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public partial class UserLogout : ComponentBase {
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [CascadingParameter] private HttpContext HttpContext { get; set; } = default!;
    [Inject] private IdentityRedirectManager IdentityManager { get; set; } = default!;

    protected override async Task OnInitializedAsync() {
        Log.Information("Logging out user.");
        if (SignInManager is not null) {
            await SignInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            IdentityManager.RedirectTo("/");
        }
    }
}
