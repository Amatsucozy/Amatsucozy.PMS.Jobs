using System.Runtime.InteropServices;
using Amatsucozy.PMS.Jobs.Worker;
using Amatsucozy.PMS.Jobs.Worker.Core.Options;
using Serilog;

[DllImport("kernel32.dll")]
static extern bool SetConsoleTitle(string text);

if (args.Any())
{
    SetConsoleTitle($"{typeof(Worker).Assembly.GetName().Name} {args[0]}");
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog((hostBuilderContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
        })
        .ConfigureHostConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddCommandLine(args);
        })
        .ConfigureAppConfiguration(configurationBuilder => { configurationBuilder.AddUserSecrets<Worker>(); })
        .ConfigureServices((context, services) =>
        {
            services.Configure<WorkerConfigurations>(context.Configuration);
            services.PostConfigure<WorkerConfigurations>(config =>
            {
                if (config.ClientId == 0)
                {
                    throw new Exception();
                }
            });

            services.AddHostedService<Worker>();
        })
        .Build();

    host.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
    throw;
}
