using Microsoft.AspNetCore.Identity;
using Quartz;

[DisallowConcurrentExecution]
public class UserManagerJob : IJob {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    public UserManagerJob(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task Execute(IJobExecutionContext context) {
        await CreateRolesAndAdminUser();
    }
    internal async Task CreateRolesAndAdminUser() {
        const string adminRoleName = "Administrator";
        string[] roleNames = { adminRoleName, "User", "LeagueManager" };

        foreach (string roleName in roleNames) {
            await CreateRole(roleName);
        }

        // Get these value from "appsettings.json" file.
        string adminUserEmail = "markmjohnson@gmail.com";
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
            var newAppUser = new IdentityUser {
                Email = userEmail,
                UserName = userEmail
            };

            var appUser = await _userManager.CreateAsync(newAppUser);

            var newUserRole = await _userManager.AddToRoleAsync(newAppUser, roleName);
        }
        else {
            var newUserRole = await _userManager.AddToRoleAsync(checkAppUser, roleName);
        }
    }

}