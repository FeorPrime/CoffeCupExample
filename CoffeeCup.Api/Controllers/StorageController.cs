using Asp.Versioning;
using CoffeeCup.Contracts;
using CoffeeCup.Contracts.Resources;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCup.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class StorageController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public StorageController( IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet(ApiEndpoints.Storage.CheckResources)]
    public async Task<IActionResult> CheckResourcesAsync([FromServices] IRequestClient<CheckResources> client,
        CancellationToken cancellationToken)
    {
        var response = await client.GetResponse<ResourcesReport>(new CheckResources(Guid.NewGuid()), cancellationToken);
        return Ok(response.Message.ResourceDto);
    }

    [HttpDelete(ApiEndpoints.Storage.PruneStorage)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PruneResourcesAsync(CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new PruneResources(Guid.NewGuid()), cancellationToken);
        return Accepted();
    }

    [HttpPost(ApiEndpoints.Storage.TakeResources)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TakeResourcesAsync([FromServices] IRequestClient<TakeResources> client, ResourcesDto request, CancellationToken cancellationToken)
    {
        var response = (await client.GetResponse<TakeResources>(new TakeResources(Guid.NewGuid(), request), cancellationToken)).Message.Result 
                       ?? throw new Exception("Could not take resources");
        if (response) return Ok();
        return BadRequest("Storage is out of resources");
    }

    [HttpPost(ApiEndpoints.Storage.StoreResources)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> StoreResourcesAsync([FromServices] IRequestClient<StoreResources> client,[FromBody] ResourcesDto request, 
        CancellationToken cancellationToken)
    {
        await client.GetResponse<StoreResources>(new StoreResources(Guid.NewGuid(), request), cancellationToken); 
        return Accepted();
    }
}