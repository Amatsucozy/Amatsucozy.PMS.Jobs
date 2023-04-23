using System.Collections.Concurrent;
using System.Diagnostics;
using Amatsucozy.PMS.Jobs.Orchestrator.Core.Options;
using Microsoft.Extensions.Options;
using Serilog;

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
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(_options.CurrentValue.WorkerExecutablePath),
                ArgumentList =
                {
                    $"ClientId={clientId}"
                }
            }
        };

        Log.Information("Starting worker for client {clientId}", clientId);
        workerProcess.Start();
        Log.Information("Worker for client {clientId} started", clientId);

        return workerProcess.Id;
    }

    public void Stop(int clientId)
    {
        Log.Information("Getting worker id for client {clientId}", clientId);
        var getValueSucceeded = ClientWorkerDictionary.TryGetValue(clientId, out var workerProcessId);

        if (!getValueSucceeded)
        {
            _logger.LogWarning("No worker found for client {clientId}", clientId);
            return;
        }

        Log.Information("Stopping worker {workerProcessId} for client {clientId}", workerProcessId, clientId);
        var workerProcess = Process.GetProcessById(workerProcessId);
        workerProcess.Kill();
        Log.Information("Worker {workerProcessId} for client {clientId} stopped", workerProcessId, clientId);
    }

    public void AddClientWorker(int clientId, int workerProcessId)
    {
        Log.Information("Adding client {clientId} with worker {workerProcessId}", clientId, workerProcessId);
        var addSucceeded = ClientWorkerDictionary.TryAdd(clientId, workerProcessId);

        if (!addSucceeded)
        {
            _logger.LogWarning("Client {clientId} already exists", clientId);
            return;
        }

        Log.Information("Client {clientId} with worker {workerProcessId} added", clientId, workerProcessId);
    }

    public void RemoveClientWorker(int clientId)
    {
        Log.Information("Removing client {clientId}", clientId);
        var removeSucceeded = ClientWorkerDictionary.TryRemove(clientId, out _);

        if (!removeSucceeded)
        {
            _logger.LogWarning("Client {clientId} does not exist", clientId);
            return;
        }

        Log.Information("Client {clientId} removed", clientId);
    }
}
