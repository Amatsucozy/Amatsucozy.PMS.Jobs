namespace Amatsucozy.PMS.Jobs.Orchestrator.Core.Options;

public sealed class JobWorkerConfigurations
{
    public required string WorkerExecutablePath { get; set; }
    
    public required string WorkerName { get; set; }
}
