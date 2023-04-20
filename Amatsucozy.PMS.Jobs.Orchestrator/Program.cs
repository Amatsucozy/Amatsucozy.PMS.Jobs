using Amatsucozy.PMS.Jobs.Orchestrator;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Orchestrator>(); })
    .Build();

host.Run();