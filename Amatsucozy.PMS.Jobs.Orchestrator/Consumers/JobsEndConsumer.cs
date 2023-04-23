using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using MassTransit;
using Serilog;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Consumers;

public sealed class JobsEndConsumer : IConsumer<EndJobNotification>
{
    private readonly ILogger<JobsEndConsumer> _logger;
    private readonly OrchestratorService _service;

    public JobsEndConsumer(ILogger<JobsEndConsumer> logger, OrchestratorService service)
    {
        _logger = logger;
        _service = service;
    }

    public Task Consume(ConsumeContext<EndJobNotification> context)
    {
        try
        {
            Log.Information("Received end job notification for client {clientId}", context.Message.ClientId);
            _service.Stop(context.Message.ClientId);
            _service.RemoveClientWorker(context.Message.ClientId);
            Log.Information("Stopped worker for client {clientId}", context.Message.ClientId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while stopping worker for client {clientId}", context.Message.ClientId);
            throw;
        }

        return Task.CompletedTask;
    }
}
