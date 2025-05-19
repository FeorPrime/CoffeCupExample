using CoffeeCup.Contracts.Resources;
using CoffeeCup.Storage.Services;
using MassTransit;

namespace CoffeeCup.Storage.Consumers;

public class PruneResourcesConsumer : IConsumer<PruneResources>
{
    private readonly StorageService _storageService;

    public PruneResourcesConsumer(StorageService storageService) { _storageService = storageService; }
    public async Task Consume(ConsumeContext<PruneResources> context)
    {
        await _storageService.PruneResourcesAsync(context.CancellationToken);
    }
}

public class PruneResourcesFaultConsumer : IConsumer<Fault<PruneResources>>
{
    private readonly ILogger<PruneResourcesFaultConsumer> _logger;

    public PruneResourcesFaultConsumer(ILogger<PruneResourcesFaultConsumer> logger) { _logger = logger; }
    public Task Consume(ConsumeContext<Fault<PruneResources>> context)
    {
        _logger.LogError("{@Message}", context.Message);
        return Task.CompletedTask;
    }
}