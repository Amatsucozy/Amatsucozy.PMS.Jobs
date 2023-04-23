using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using MassTransit;
using Serilog;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Consumers;

public sealed class JobsStartConsumer : IConsumer<StartJobNotification>
{
    private readonly ILogger<JobsStartConsumer> _logger;
    private readonly OrchestratorService _service;

    public JobsStartConsumer(ILogger<JobsStartConsumer> logger, OrchestratorService service)
    {
        _logger = logger;
        _service = service;
    }

    public Task Consume(ConsumeContext<StartJobNotification> context)
    {
        try
        {
            Log.Information("Received start job notification for client {clientId}", context.Message.ClientId);
            var workerProcessId = _service.Start(context.Message.ClientId);
            _service.AddClientWorker(context.Message.ClientId, workerProcessId);
            Log.Information("Started worker for client {clientId}", context.Message.ClientId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while starting worker for client {clientId}", context.Message.ClientId);
            throw;
        }

        return Task.CompletedTask;
    }
}
