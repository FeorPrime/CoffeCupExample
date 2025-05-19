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
