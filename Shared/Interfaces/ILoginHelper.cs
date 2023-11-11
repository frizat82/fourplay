using Microsoft.AspNetCore.Identity;

public interface ILoginHelper {
    Task<IdentityUser?> GetUserDetails();
}