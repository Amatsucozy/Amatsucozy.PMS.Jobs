using System.Collections.Concurrent;
using System.Diagnostics;
using Amatsucozy.PMS.Jobs.Orchestrator.Core.Options;
using Microsoft.Extensions.Options;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Services;

public sealed class OrchestratorService
{
    private readonly ILogger<OrchestratorService> _logger;
    private readonly IOptionsMonitor<JobWorkerConfigurations> _options;

    private static readonly ConcurrentDictionary<int, int> ClientWorkerMapping = new();

    public OrchestratorService(ILogger<OrchestratorService> logger, IOptionsMonitor<JobWorkerConfigurations> options)
    {
        _logger = logger;
        _options = options;

        PopulateClientWorkerMapping();
    }

    private IEnumerable<Process> ListAllJobs()
    {
        return Process.GetProcesses().Where(p => p.ProcessName == _options.CurrentValue.WorkerName);
    }

    private void PopulateClientWorkerMapping()
    {
        if (!ClientWorkerMapping.IsEmpty)
        {
            return;
        }

        foreach (var process in ListAllJobs())
        {
            ClientWorkerMapping.TryAdd(process.Id, process.Id);
        }
    }

    public int Start(int clientId)
    {
        var workerProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _options.CurrentValue.WorkerExecutablePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                ArgumentList =
                {
                    clientId.ToString()
                }
            }
        };
        _logger.LogInformation("Starting worker for client {clientId}", clientId);
        workerProcess.Start();
        _logger.LogInformation("Worker for client {clientId} started", clientId);

        return workerProcess.Id;
    }

    public void Stop(int clientId)
    {
        var getValueSucceeded = ClientWorkerMapping.TryGetValue(clientId, out var workerId);

        if (!getValueSucceeded)
        {
            _logger.LogWarning("No worker found for client {clientId}", clientId);
            return;
        }

        _logger.LogInformation("Stopping worker {workerId} for client {clientId}", workerId, clientId);
        var workerProcess = Process.GetProcessById(workerId);
        workerProcess.Kill();
        _logger.LogInformation("Worker {workerId} for client {clientId} stopped", workerId, clientId);
    }

    public void AddClientWorker(int clientId, int processId)
    {
        _logger.LogInformation("Adding client {clientId} with worker {workerId}", clientId, processId);
        var addSucceeded = ClientWorkerMapping.TryAdd(clientId, processId);

        if (!addSucceeded)
        {
            _logger.LogWarning("Client {clientId} already exists", clientId);
            return;
        }

        _logger.LogInformation("Client {clientId} with worker {workerId} added", clientId, processId);
    }
}
