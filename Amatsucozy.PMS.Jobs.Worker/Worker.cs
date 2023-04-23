using Amatsucozy.PMS.Jobs.Worker.Core.Options;
using Microsoft.Extensions.Options;

namespace Amatsucozy.PMS.Jobs.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<WorkerConfigurations> _options;

    public Worker(ILogger<Worker> logger, IOptions<WorkerConfigurations> options)
    {
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(10000, stoppingToken);
        }
    }
}