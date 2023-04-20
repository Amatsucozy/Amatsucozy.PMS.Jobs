using Amatsucozy.PMS.Jobs.Orchestrator;
using Amatsucozy.PMS.Jobs.Orchestrator.Core.Options;
using Amatsucozy.PMS.Jobs.Orchestrator.Services;
using Amatsucozy.PMS.Shared.Helpers.MessageQueues;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((hostBuilderContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
    })
    .ConfigureHostConfiguration(configurationBuilder => { configurationBuilder.AddEnvironmentVariables(); })
    .ConfigureAppConfiguration(configurationBuilder => { configurationBuilder.AddUserSecrets<Orchestrator>(); })
    .ConfigureServices((context, services) =>
    {
        services.Configure<JobWorkerConfigurations>(context.Configuration.GetSection(nameof(JobWorkerConfigurations)));
        services.PostConfigure<JobWorkerConfigurations>(config =>
        {
            if (string.IsNullOrWhiteSpace(config.WorkerExecutablePath))
            {
                throw new Exception();
            }

            if (string.IsNullOrWhiteSpace(config.WorkerName))
            {
                throw new Exception();
            }
        });

        services.AddSingleton<OrchestratorService>();
        services.AddMessageQueue(context.Configuration, typeof(Orchestrator));

        services.AddHostedService<Orchestrator>();
    })
    .Build();

host.Run();
