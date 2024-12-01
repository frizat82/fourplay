using fourplay.Data;
using Microsoft.AspNetCore.Identity;

public interface ILoginHelper {
    Task<ApplicationUser?> GetUserDetails();
}