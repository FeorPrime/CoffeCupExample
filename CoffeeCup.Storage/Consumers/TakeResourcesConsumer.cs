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
