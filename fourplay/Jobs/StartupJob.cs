using fourplay.Data;
using Quartz;
namespace fourplay.Jobs;
public class StartupJob : IJob
{
    private readonly ISchedulerFactory _factory;
    private readonly ApplicationDbContext _context;
    public StartupJob(ISchedulerFactory factory)
    {
        _factory = factory;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = await _factory.GetScheduler();
        await scheduler.TriggerJob(new JobKey("User Manager"));
        await Task.Delay(TimeSpan.FromMinutes(1));
        await scheduler.TriggerJob(new JobKey("NFL Scores"));
        await Task.Delay(TimeSpan.FromMinutes(1));
        await scheduler.TriggerJob(new JobKey("NFL Spreads"));
    }
}