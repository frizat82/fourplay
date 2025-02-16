using Microsoft.AspNetCore.Identity;

namespace fourplay.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public string FullName { get; set; }
    public string NickName { get; set; }
}