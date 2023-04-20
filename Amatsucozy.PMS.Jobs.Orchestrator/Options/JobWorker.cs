namespace Amatsucozy.PMS.Jobs.Orchestrator.Options;

public sealed class JobWorker
{
    public required string WorkerExecutablePath { get; set; }
    
    public required string WorkerName { get; set; }
}