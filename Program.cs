using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fourplay.Data;
using Quartz;
using Quartz.Impl.AdoJobStore;
using MudBlazor.Services;
using System.Data.SQLite;
using Serilog;
using Blazored.LocalStorage;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) => configuration
                .WriteTo.Console()
                    .MinimumLevel.Override("Quartz", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
                .ReadFrom.Services(services));
builder.Services.AddMudServices();
// Listen 
//builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(new SQLiteConnection(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
}).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
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
    googleOptions.Events.OnCreatingTicket = (context) => {
        var picture = context.User.GetProperty("picture").GetString();
        context.Identity?.AddClaim(new Claim("picture", picture));
        return Task.CompletedTask;
    };
    // Only if we use GOOGLE FALLBACK
    googleOptions.Events.OnTicketReceived = async context => {
        void AccessDenied() {
            context.Response.Redirect("/Auth/Unauthorized");
            context.Fail("You are not allowed");
            context.HandleResponse();
        }
        var roleManager = context.HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        var result = await roleManager.RoleExistsAsync("Administrator");
        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
        if (result) {
            claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
        }
        context.Success();
    };
    /*
    googleOptions.Events.OnRedirectToAuthorizationEndpoint = context => {
        // this will avoid automatic re-login when signing out
        context.Response.Redirect(context.RedirectUri + "&prompt=consent");
        return Task.CompletedTask;
    };*/
});
builder.Services.AddAuthorization(options =>
      options.AddPolicy("User",
      policy => policy.RequireClaim(ClaimTypes.Email, new[] { "markmjohnson@gmail.com" })));
/*
// If this is done we need to add ROLE support above
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder(
            GoogleDefaults.AuthenticationScheme
        )
        .RequireAuthenticatedUser()
        .RequireClaim(ClaimTypes.Email, new[] { "markmjohnson@gmail.com" })
        .Build()
);*/
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ILoginHelper, LoginHelper>();
// Quartz
builder.Services.AddScoped<IJob, NFLScoresJob>();
builder.Services.AddScoped<IJob, NFLSpreadJob>();
builder.Services.AddScoped<IJob, StartupJob>();
builder.Services.AddScoped<IJob, UserManagerJob>();
builder.Services.AddQuartz(q => {
    q.UseMicrosoftDependencyInjectionJobFactory();
    // quickest way to create a job with single trigger is to use ScheduleJob
    // (requires version 3.2)
    q.ScheduleJob<UserManagerJob>(trigger => trigger.WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).WithRepeatCount(0)).StartNow());
    q.ScheduleJob<StartupJob>(trigger => trigger.WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).WithRepeatCount(0)).StartNow());
    q.ScheduleJob<NFLSpreadJob>(trigger => trigger
        .WithIdentity("NFL Spreads")
        .WithCronSchedule("0 0 14 ? * THU", x => x.WithMisfireHandlingInstructionFireAndProceed()) // Fire at 10:00 AM every Thursday

    );
    q.ScheduleJob<NFLScoresJob>(trigger => trigger
    .WithIdentity("NFL Scores")
    .WithCronSchedule("0 0 10 ? * THU", x => x.WithMisfireHandlingInstructionFireAndProceed()) // Fire at 10:00 AM every Thursday
);
    /*
    // convert time zones using converter that can handle Windows/Linux differences
    q.UseTimeZoneConverter();

    // auto-interrupt long-running job
    q.UseJobAutoInterrupt(options =>
    {
        // this is the default
        options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
    });
    */
    // example of persistent job store using JSON serializer as an example

    q.UsePersistentStore(s => {
        // Use for SQLite database
        s.UseSQLite(sqlLiteOptions => {
            sqlLiteOptions.UseDriverDelegate<SQLiteDelegate>();
            sqlLiteOptions.ConnectionString = connectionString;
            sqlLiteOptions.TablePrefix = "QRTZ_";
        });
        s.PerformSchemaValidation = true; // default
        s.UseProperties = true; // preferred, but not default
        s.RetryInterval = TimeSpan.FromSeconds(15);
        s.UseNewtonsoftJsonSerializer();
        /*s.UseClustering(c =>
        {
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
            c.CheckinInterval = TimeSpan.FromSeconds(10);
        });*/
    });

});

// Quartz.Extensions.Hosting allows you to fire background service that handles scheduler lifecycle
builder.Services.AddQuartzHostedService(options => {
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.Run();
