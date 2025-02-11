using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using fourplay.Components.Account;
using fourplay.Data;
using Quartz;
using Quartz.Impl.AdoJobStore;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using fourplay.Jobs;
using Serilog.Formatting.Compact;
using fourplay.Services.Interfaces;
using fourplay.Services;
using fourplay.Components;
Environment.SetEnvironmentVariable("DOTNET_hostBuilder:reloadConfigOnChange", "false");
var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .WriteTo.Console(new CompactJsonFormatter()).Enrich.FromLogContext()
    .MinimumLevel.Override("Quartz", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
    .ReadFrom.Services(services));

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING") ??
                       throw new InvalidOperationException("Connection string 'AZURE_POSTGRESQL_CONNECTIONSTRING' not found.");
Log.Information("DB {ConnectionString}", connectionString);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => {
    //Log.Information("DB {ConnectionString}", connectionString);
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(true);
    options.EnableDetailedErrors(true);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<ISpreadCalculator, SpreadCalculator>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddHttpClient<IESPNApiService, ESPNApiService>(x => {
    x.BaseAddress = new Uri("http://site.api.espn.com");
});
builder.Services.AddHttpClient<ISportslineOddsService, SportslineOddsService>(x => {
    x.BaseAddress = new Uri("https://www.sportsline.com");
    // Set headers
    x.DefaultRequestHeaders.Add("authority", "www.sportsline.com");
    x.DefaultRequestHeaders.Add("accept", "*/*");
    x.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
    x.DefaultRequestHeaders.Add("cookie", "fly_device=desktop; fly_geo={\"countryCode\":\"us\",\"region\":\"il\",\"dma\":\"602\"}; fly_ab_uid=34aa0b5c-1722-45ab-bad9-cf062b209d50; OptanonAlertBoxClosed=2023-10-20T00:07:41.540Z; OptanonConsent=isIABGlobal=false&datestamp=Thu+Oct+19+2023+19%3A07%3A42+GMT-0500+(Central+Daylight+Time)&version=6.30.0&hosts=&genVendors=V16%3A0%2CV10%3A0%2CV12%3A0%2CV9%3A0%2CV15%3A0%2CV6%3A0%2CV8%3A0%2CV5%3A0%2CV7%3A0%2CV11%3A0%2C&consentId=d11e2960-854f-4fe1-8ac1-d67e3052ab87&interactionCount=1&landingPath=NotLandingPage&groups=1%3A1%2C2%3A1%2C3%3A1%2C4%3A1%2C5%3A1&AwaitingReconsent=false&geolocation=%3B");
    x.DefaultRequestHeaders.Add("origin", "https://www.sportsline.com");
    x.DefaultRequestHeaders.Add("referer", "https://www.sportsline.com/nfl/odds/");
    x.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"118\", \"Google Chrome\";v=\"118\", \"Not=A?Brand\";v=\"99\"");
    x.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
    x.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Linux\"");
    x.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
    x.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
    x.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
    x.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");
    x.DefaultRequestHeaders.Add("x-correlation-id", "51f96c9b-8cf4-4e98-9460-d1c0025198c0");
});
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication().AddGoogle(googleOptions => {
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    // Add Picture Support
    googleOptions.Scope.Add("profile");
    googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    googleOptions.Events.OnCreatingTicket = (context) => {
        var picture = context.User.GetProperty("picture").GetString();
        if (picture is not null)
            context.Identity?.AddClaim(new Claim("urn:google:picture", picture));
        return Task.CompletedTask;
    };
    // Only if we use GOOGLE FALLBACK
    googleOptions.Events.OnTicketReceived = async context => {
        // Resolve the DbContext from the IServiceProvider
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var db = dbContext.CreateDbContext();
        var validEmails = db.LeagueUsers.Select(x => x.GoogleEmail).ToList();
        async Task AccessDenied(string email) {
            Log.Warning("Logging in {User} Denied", email);
            await context.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            context.Response.Redirect("/auth");
            context.Fail("You are not allowed");
            context.HandleResponse();
        }
        var roleManager = context.HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        var result = await roleManager.RoleExistsAsync("Administrator");
        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
        var email = claimsIdentity.Claims.FirstOrDefault(x => x.Type.Contains("email"));
        if (email is null) {
            await AccessDenied(email.Value);
            return;
        }
        if (result) {
            if (email.Value == "markmjohnson@gmail.com") {
                claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
                Log.Information("Logging in {User} Successfully as Admin", email.Value);
            }
        }
        if (!validEmails.Contains(email.Value)) {
            await AccessDenied(email.Value);
        }
        else {
            Log.Information("Logging in {User} Successfully", email.Value);
            context.Success();
        }
    };
    /*
    googleOptions.Events.OnRedirectToAuthorizationEndpoint = context => {
        // this will avoid automatic re-login when signing out
        context.Response.Redirect(context.RedirectUri + "&prompt=consent");
        return Task.CompletedTask;
    };*/
});
/*
builder.Services.AddAuthorization(options =>
      options.AddPolicy("User",
      policy => policy.RequireClaim(ClaimTypes.Email, new[] { "markmjohnson@gmail.com", "jpmulcahy@gmail.com" })));

*/
builder.Services.AddAuthorization();
/*
// If this is done we need to add ROLE support above
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder(
            GoogleDefaults.AuthenticationScheme
        )
        .RequireAuthenticatedUser()
        .RequireClaim(ClaimTypes.Email, new[] { "markmjohnson@gmail.com","jpmulcahy@gmail.com" })
        .Build()
);
*/
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<ILoginHelper, LoginHelper>();
// Quartz
builder.Services.AddScoped<IJob, NFLScoresJob>();
builder.Services.AddScoped<IJob, NFLSpreadJob>();
builder.Services.AddScoped<IJob, StartupJob>();
builder.Services.AddScoped<IJob, UserManagerJob>();
builder.Services.AddQuartz(q => {
    //q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<UserManagerJob>(trigger => trigger
        .WithIdentity("User Manager")
            .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Minute))
            ); // use DateBuilder to create a date in the future
    q.ScheduleJob<NFLSpreadJob>(trigger => trigger
        .WithIdentity("NFL Spreads")
        .WithCronSchedule("0 0 10 24 12 ?", x => x.WithMisfireHandlingInstructionFireAndProceed()) // Fire at 10:00 AM every xmas eve
        .WithCronSchedule("0 0 14 ? * THU", x => x.WithMisfireHandlingInstructionFireAndProceed()) // Fire at 2:00 PM every Thursday

    );
    q.ScheduleJob<NFLScoresJob>(trigger => trigger
    .WithIdentity("NFL Scores")
    .WithCronSchedule("0 0 10 ? * THU", x => x.WithMisfireHandlingInstructionFireAndProceed()) // Fire at 10:00 AM every Thursday
);
    q.UsePersistentStore(s => {
        // Use for Postgres database
        s.UsePostgres(postGresOptions => {
            postGresOptions.UseDriverDelegate<PostgreSQLDelegate>();
            postGresOptions.ConnectionString = connectionString;
            postGresOptions.TablePrefix = "quartz.qrtz_";
        });
        s.PerformSchemaValidation = true; // default
        s.UseProperties = true; // preferred, but not default
        s.RetryInterval = TimeSpan.FromSeconds(15);
        s.UseNewtonsoftJsonSerializer();
    });
});

// Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
builder.Services.AddQuartzHostedService(options => {
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});


var app = builder.Build();

try {
    Log.Information("Migrating any needed DBs");
    using (var scope = app.Services.CreateScope()) {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
catch (Exception ex) {
    Log.Error(ex, "Error Upgrading DB");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();