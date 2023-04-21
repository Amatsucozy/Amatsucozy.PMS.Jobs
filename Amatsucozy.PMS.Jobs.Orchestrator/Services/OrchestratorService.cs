using System.Collections.Concurrent;
using System.Diagnostics;
using Amatsucozy.PMS.Jobs.Orchestrator.Core.Options;
using Microsoft.Extensions.Options;

namespace Amatsucozy.PMS.Jobs.Orchestrator.Services;

public sealed class OrchestratorService
{
    private readonly ILogger<OrchestratorService> _logger;
    private readonly IOptionsMonitor<JobWorkerConfigurations> _options;

    private static readonly ConcurrentDictionary<int, int> ClientWorkerDictionary = new();

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
        if (!ClientWorkerDictionary.IsEmpty)
        {
            return;
        }

        foreach (var process in ListAllJobs())
        {
            ClientWorkerDictionary.TryAdd(process.Id, process.Id);
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
        _logger.LogInformation("Getting worker id for client {clientId}", clientId);
        var getValueSucceeded = ClientWorkerDictionary.TryGetValue(clientId, out var workerProcessId);

        if (!getValueSucceeded)
        {
            _logger.LogWarning("No worker found for client {clientId}", clientId);
            return;
        }

        _logger.LogInformation("Stopping worker {workerProcessId} for client {clientId}", workerProcessId, clientId);
        var workerProcess = Process.GetProcessById(workerProcessId);
        workerProcess.Kill();
        _logger.LogInformation("Worker {workerProcessId} for client {clientId} stopped", workerProcessId, clientId);
    }

    public void AddClientWorker(int clientId, int workerProcessId)
    {
        _logger.LogInformation("Adding client {clientId} with worker {workerProcessId}", clientId, workerProcessId);
        var addSucceeded = ClientWorkerDictionary.TryAdd(clientId, workerProcessId);

        if (!addSucceeded)
        {
            _logger.LogWarning("Client {clientId} already exists", clientId);
            return;
        }

        _logger.LogInformation("Client {clientId} with worker {workerProcessId} added", clientId, workerProcessId);
    }
    
    public void RemoveClientWorker(int clientId)
    {
        _logger.LogInformation("Removing client {clientId}", clientId);
        var removeSucceeded = ClientWorkerDictionary.TryRemove(clientId, out _);

        if (!removeSucceeded)
        {
            _logger.LogWarning("Client {clientId} does not exist", clientId);
            return;
        }

        _logger.LogInformation("Client {clientId} removed", clientId);
    }
}