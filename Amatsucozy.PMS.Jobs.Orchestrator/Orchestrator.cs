using Serilog;

namespace Amatsucozy.PMS.Jobs.Orchestrator;

public class Orchestrator : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        Log.Information("Orchestrator is stopping.");
    }
}
