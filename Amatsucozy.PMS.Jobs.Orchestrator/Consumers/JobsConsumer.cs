using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using MassTransit;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Consumers;

public sealed class JobsConsumer : IConsumer<JobInfo>
{
    private readonly ILogger<JobsConsumer> _logger;
    private readonly OrchestratorService _service;

    public JobsConsumer(ILogger<JobsConsumer> logger, OrchestratorService service)
    {
        _logger = logger;
        _service = service;
    }

    public Task Consume(ConsumeContext<JobInfo> context)
    {
        var processId = _service.Start(context.Message.ClientId);
        _service.AddClientWorker(context.Message.ClientId, processId);

        return Task.CompletedTask;
    }
}
