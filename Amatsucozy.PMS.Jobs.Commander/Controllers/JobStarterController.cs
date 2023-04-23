using Amatsucozy.PMS.Jobs.Orchestrator.Core;
using Amatsucozy.PMS.Shared.API.Controllers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Amatsucozy.PMS.Jobs.Commander.Controllers;

public sealed class JobStarterController : PublicController
{
    private readonly IPublishEndpoint _publishEndpoint;
    private static readonly Random Rng = new();

    public JobStarterController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> StartJob(CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new StartJobNotification
        {
            ClientId = Rng.Next(1, 3000)
        }, cancellationToken);

        return Ok();
    }
}
