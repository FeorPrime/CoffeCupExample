using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using TestB.Contracts;

namespace TestB.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheEventController: ControllerBase
{
    [HttpGet("publish10")]
    public async Task<IActionResult> Publish10([FromServices] IPublishEndpoint bus, [FromServices] LoadBalancer lb)
    {
        for (int i = 0; i < 10; i++)
        {
            var route = lb.GetRouteKeyByRoundRobin().ToUpper();
            var payload = new TheEvent(Guid.NewGuid(), route);
            Thread.Sleep(100);
            await bus.Publish(payload, c =>
            {
                c.SetRoutingKey($"{route}");
            });
        }

        return Accepted();
    }
    
    [HttpGet("send10")]
    public async Task<IActionResult> Send10([FromServices] ISendEndpoint bus, [FromServices] LoadBalancer lb)
    {
        for (int i = 0; i < 10; i++)
        {
            //var route = lb.GetQueueByRoundRobin();
            var route = lb.GetRouteKeyByRoundRobin().ToUpper();
            
            var payload = new TheEvent(Guid.NewGuid(), route);
            
            Thread.Sleep(100);
            
            // var ep = await bus.GetSendEndpoint(new Uri($"queue:{route}"));
            // await ep.Send(payload, s =>
            // {
            //     s.SetRoutingKey("A");
            // });

            await bus.Send(payload, s =>
            {
                s.SetRoutingKey($"{route}");
            });
        }

        return Accepted();
    }
}