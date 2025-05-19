using CoffeeCup.Contracts.Resources;
using CoffeeCup.Storage.Services;
using MassTransit;

namespace CoffeeCup.Storage.Consumers;

public class TakeResourcesConsumer : IConsumer<TakeResources>
{
    private readonly StorageService _storageService;

    public TakeResourcesConsumer(StorageService storageService) { _storageService = storageService; }
    public async Task Consume(ConsumeContext<TakeResources> context)
    {
        var result = await _storageService.TakeResourcesAsync(context.Message.ResourceDto, context.CancellationToken);
        await context.RespondAsync(context.Message with { Result = result });
    }
}

public class TakeResourcesFaultConsumer : IConsumer<Fault<TakeResources>>
{
    private readonly ILogger<TakeResourcesFaultConsumer> _logger;

    public TakeResourcesFaultConsumer(ILogger<TakeResourcesFaultConsumer> logger) { _logger = logger; }
    public Task Consume(ConsumeContext<Fault<TakeResources>> context)
    {
        _logger.LogError("{@Message}", context.Message);
        return Task.CompletedTask;
    }
}