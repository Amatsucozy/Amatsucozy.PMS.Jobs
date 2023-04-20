namespace Amatsucozy.PMS.Jobs.Orchestrator;

public class Orchestrator : BackgroundService
{
    private readonly ILogger<Orchestrator> _logger;

    public Orchestrator(ILogger<Orchestrator> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}