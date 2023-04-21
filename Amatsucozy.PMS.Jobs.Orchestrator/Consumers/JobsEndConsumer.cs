using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using MassTransit;

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
        _service.Stop(context.Message.ClientId);
        _service.RemoveClientWorker(context.Message.ClientId);

        return Task.CompletedTask;
    }
}