using CoffeeCup.Contracts.Resources;
using CoffeeCup.Storage.Services;
using MassTransit;

namespace CoffeeCup.Storage.Consumers;

public class CheckResourcesConsumer : IConsumer<CheckResources>
{
    private readonly StorageService _storageService;
    public CheckResourcesConsumer(StorageService storageService) { _storageService = storageService; }
    
    public async Task Consume(ConsumeContext<CheckResources> context)
    {
        var result = await _storageService.CheckResourcesAsync(context.CancellationToken);
        await context.RespondAsync(new ResourcesReport(context.Message.CorrelationId, result));
    }
}
