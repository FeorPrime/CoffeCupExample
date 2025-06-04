using contracts;
using contracts.Contracts;
using contracts.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagerController: ControllerBase
{
    private readonly IPublishEndpoint _publish;

    public MessagerController(IPublishEndpoint publish) { _publish = publish; }

    [HttpPost("/start")]
    public IActionResult SendStep1([FromServices] LoadBalancerHelper loadBalancerHelper)
    {
        var id = Guid.NewGuid();
        
        STMHelper.PrintPretty($"Starting new machine : {id.ToString()}");
        var payload = new StartMachine(id, loadBalancerHelper.GetExchange());
        
        _publish.Publish(payload);
        
        return Accepted();
    }

    [HttpPost("/start10")]
    public IActionResult SendStep2([FromServices] LoadBalancerHelper loadBalancerHelper)
    {
        for (int i = 0; i < 10; i++)
        {
            var id = Guid.NewGuid();
        
            STMHelper.PrintPretty($"Starting new machine : {id.ToString()}");
            _publish.Publish(new StartMachine(id, loadBalancerHelper.GetExchange()));
        }
        //STMHelper.PrintPretty($"Message sent: {id.ToString()}");
        return Accepted();
    }

    [HttpGet("/state")]
    public async Task<IActionResult> GetState([FromServices] ExpContext ctx)
    {
        var state = await ctx.ProcessingStates.AsNoTracking().Take(20).ToListAsync();
        return Ok(state);
    }
    
    [HttpGet("/reports")]
    public async Task<IActionResult> GetReports([FromServices] ExpContext ctx)
    {
        var reports = await ctx.WorkReports.AsNoTracking().Take(100).ToListAsync();
        return Ok(reports);
    }

    [HttpGet("/send10")]
    public async Task<IActionResult> Send10([FromServices]IBus bus)
    {
        const string sendA = "queue:machine-events-alpha";
        const string sendB = "queue:machine-events-beta";
        var uriA = new Uri(sendA);
        var uriB = new Uri(sendB);
        
        var state = false;

        for (var i = 0; i < 10; i++)
        {
            var payload = new StartMachine(Guid.NewGuid(), state ? sendA : sendB);
            var ep = await bus.GetSendEndpoint(state ? uriA : uriB);
            await ep.Send(payload, s =>
            {
                s.SetRoutingKey(state ? "machine-events-alpha" : "machine-events-beta");
            });
            state = !state;
        }

        return Accepted();
    }
}