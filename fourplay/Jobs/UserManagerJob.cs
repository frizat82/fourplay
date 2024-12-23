using System.Diagnostics;
using fourplay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Bogus;
namespace fourplay.Jobs;
[DisallowConcurrentExecution]
public class UserManagerJob : IJob
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    public UserManagerJob(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _db = db;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Information("Executing UserManagerJob");
        await CreateRolesAndAdminUser();
    }
    internal async Task CreateRolesAndAdminUser()
    {
        Log.Information("Adding roles and assigning Admins");
        const string adminRoleName = "Administrator";
        string[] roleNames = { adminRoleName, "User", "LeagueManager" };

        foreach (var roleName in roleNames)
        {
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
    }
    /// <summary>
    /// Create base user
    /// </summary>
    internal async Task CreateBaseUser(string userEmail)
    {
        var user = new LeagueUsers() { GoogleEmail = userEmail };
        await _db.LeagueUsers.AddAsync(user);
        await _db.SaveChangesAsync();
        Log.Information("Base user created {@Identity}", user);
    }

    /// <summary>
    /// Create a role if not exists.
    /// </summary>
    /// <param name="serviceProvider">Service Provider</param>
    /// <param name="roleName">Role Name</param>
    internal async Task CreateRole(string roleName)
    {

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
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
        string roleName)
    {

        var checkAppUser = await _userManager.FindByEmailAsync(userEmail);

        if (checkAppUser is null || checkAppUser.Id is null)
        {
            return;
        }
        else
        {
            var newUserRole = await _userManager.AddToRoleAsync(checkAppUser, roleName);
            Log.Information("User Added To Role {@Identity}", newUserRole);
        }
    }
    /*
    internal async Task PopulateDatabaseWithRandomData()
    {
        var faker = new Faker();

        // Add random users
        var currentUsers = await _db.LeagueUsers.ToListAsync();
        if (currentUsers.Count < 10)
        {
            for (int i = 0; i < 10 - urrentUsers.Count; i++)
            {
                var user = new LeagueUsers
                {
                    GoogleEmail = faker.Internet.Email()
                };
                await _db.LeagueUsers.AddAsync(user);
            }

        }
        await _db.SaveChangesAsync();

        // Add random scores
        var users = await _db.LeagueUsers.ToListAsync();


        await _db.SaveChangesAsync();

        // Add random spreads
        foreach (var gameId in games)
        {
            var spread = new NFLSpreads
            {
                GameId = gameId,
                Spread = faker.Random.Double(-10, 10)
            };
            await _db.NFLSpreads.AddAsync(spread);
        }

        await _db.SaveChangesAsync();
    }
*/
}