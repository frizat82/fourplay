using System.Diagnostics;
using fourplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Bogus;
using Npgsql;
namespace fourplay.Jobs;
[DisallowConcurrentExecution]
public class UserManagerJob : IJob {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    public UserManagerJob(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext db) {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }
    public async Task Execute(IJobExecutionContext context) {
        Log.Information("Executing UserManagerJob");
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
        await CreateBaseUser(adminUserEmail);
        await AddUserToRole(adminUserEmail, adminRoleName);
        /*
        if (Debugger.IsAttached)
        {
            await PopulateDatabaseWithRandomData();
        }
        */
        Log.Information("Updatting Base 0 WeeklyCost");
        await _db.LeagueJuiceMapping.Where(b => b.WeeklyCost == 0).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.WeeklyCost, 5));
        Log.Information("Updatting Base 0 JuiceDivisional");
        await _db.LeagueJuiceMapping.Where(b => b.JuiceDivisonal == 0).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.JuiceDivisonal, 10));
        Log.Information("Updatting Base 0 JuiceConference");
        await _db.LeagueJuiceMapping.Where(b => b.JuiceConference == 0).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.JuiceConference, 6));
    }
    /// <summary>
    /// Create base user
    /// </summary>
    internal async Task CreateBaseUser(string userEmail) {
        try {
            if (await _db.LeagueUsers.AnyAsync(x => x.GoogleEmail == userEmail)) {
                return;
            }
            var user = new LeagueUsers() { GoogleEmail = userEmail };
            await _db.LeagueUsers.AddAsync(user);
            await _db.SaveChangesAsync();
            Log.Information("Base user created {@Identity}", user);
        }
        catch (Exception ex) {
            Log.Error(ex, "Uable to create base user {UserName}", userEmail);
        }
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
            Log.Information("New Role {@Identity}", result);
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
            Log.Information("User Added To Role {@Identity}", newUserRole);
        }
    }

}