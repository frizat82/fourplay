using fourplay.Data;
using fourplay.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace fourplay.Services;

public class LoginHelper : ILoginHelper {
    private ApplicationDbContext? _db { get; set; } = default!;
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    public LoginHelper(ApplicationDbContext db, AuthenticationStateProvider state) {
        _authenticationStateProvider = state;
        _db = db;
    }

    public async Task<ApplicationUser?> GetUserDetails() {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (state is null) return null;
        if (state?.User.Identity?.Name != null) {
            var email = state.User.FindFirst(ClaimTypes.Email);
            var usrId = _db.Users.FirstOrDefault(x => x.UserName == state.User.Identity.Name);
            if (usrId is null) {
                usrId = _db.Users.FirstOrDefault(x => x.Email.ToLower() == email.Value.ToLower());
            }
            return usrId;
        }
        return null;
    }
}