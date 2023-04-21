using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using MassTransit;

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
        var workerProcessId = _service.Start(context.Message.ClientId);
        _service.AddClientWorker(context.Message.ClientId, workerProcessId);

        return Task.CompletedTask;
    }
}
