using System.Collections.Concurrent;
using System.Diagnostics;
using Amatsucozy.PMS.Jobs.Orchestrator.Options;
using Microsoft.Extensions.Options;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Services;

public sealed class OrchestratorService
{
    private readonly ILogger<OrchestratorService> _logger;
    private readonly IOptionsMonitor<JobWorker> _options;

    private static readonly ConcurrentDictionary<int, int> _clientWorkerMapping = new();

    public OrchestratorService(ILogger<OrchestratorService> logger, IOptionsMonitor<JobWorker> options)
    {
        _logger = logger;
        _options = options;
    }

    public IEnumerable<Process> ListAllJobs()
    {
        return Process.GetProcesses().Where(p => p.ProcessName == _options.CurrentValue.WorkerName);
    }

    public void PopulateClientWorkerMapping()
    {
        if (_clientWorkerMapping.Count > 0)
        {
            return;
        }

        foreach (var process in ListAllJobs())
        {
            _clientWorkerMapping.TryAdd(process.Id, process.Id);
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
        workerProcess.Start();
        return workerProcess.Id;
    }

    public void Stop(int clientId)
    {
        var getValueSucceeded = _clientWorkerMapping.TryGetValue(clientId, out var workerId);
        
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
}