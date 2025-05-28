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
}