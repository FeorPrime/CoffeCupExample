using CoffeeCup.Contracts.Resources;
using CoffeeCup.Storage.Services;
using FluentValidation;
using MassTransit;

namespace CoffeeCup.Storage.Consumers;

public class StoreResourcesConsumer : IConsumer<StoreResources>
{
    private readonly StorageService _storageService;
    private readonly IValidator<ResourcesDto> _validator;

    public StoreResourcesConsumer(StorageService storageService, IValidator<ResourcesDto> validator)
    {
        _storageService = storageService;
        _validator = validator;
    }
    
    public async Task Consume(ConsumeContext<StoreResources> context)
    {
        await _validator.ValidateAndThrowAsync(context.Message.ResourceDto);
        await _storageService.StoreResourcesAsync(context.Message.ResourceDto, context.CancellationToken);
        await context.RespondAsync(context.Message);
    }
}

public class StoreResourcesErrorConsumer : IConsumer<Fault<StoreResources>>
{
    private readonly ILogger<StoreResourcesErrorConsumer> _logger;

    public StoreResourcesErrorConsumer(ILogger<StoreResourcesErrorConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<Fault<StoreResources>> context)
    {
        var fault = context.Message;
        _logger.LogError("Fault received for message {MessageId}: {Reason}", context.MessageId,
            string.Join(", ", fault.Exceptions.Select(e => e.Message)));
        
        return Task.CompletedTask;
    }
}