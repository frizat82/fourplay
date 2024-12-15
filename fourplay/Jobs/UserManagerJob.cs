using fourplay.Data;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Serilog;
namespace fourplay.Jobs;
[DisallowConcurrentExecution]
public class UserManagerJob : IJob {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public UserManagerJob(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task Execute(IJobExecutionContext context) {
        await CreateRolesAndAdminUser();
    }
    internal async Task CreateRolesAndAdminUser() {
        Log.Information("Adding roles and assigning Admins");
        const string adminRoleName = "Administrator";
        string[] roleNames = { adminRoleName, "User", "LeagueManager" };

        foreach (var roleName in roleNames) {
            await CreateRole(roleName);
        }

        // Get these value from "appsettings.json" file.
        var adminUserEmail = "markmjohnson@gmail.com";
        await AddUserToRole(adminUserEmail, adminRoleName);
    }

    /// <summary>
    /// Create a role if not exists.
    /// </summary>
    /// <param name="serviceProvider">Service Provider</param>
    /// <param name="roleName">Role Name</param>
    internal async Task CreateRole(string roleName) {

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists) {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }


    /// <summary>
    /// Add user to a role if the user exists, otherwise, create the user and adds him to the role.
    /// </summary>
    /// <param name="serviceProvider">Service Provider</param>
    /// <param name="userEmail">User Email</param>
    /// <param name="userPwd">User Password. Used to create the user if not exists.</param>
    /// <param name="roleName">Role Name</param>
    internal async Task AddUserToRole(string userEmail,
        string roleName) {

        var checkAppUser = await _userManager.FindByEmailAsync(userEmail);

        if (checkAppUser is null || checkAppUser.Id is null) {
            return;
        }
        else {
            var newUserRole = await _userManager.AddToRoleAsync(checkAppUser, roleName);
        }
    }

}