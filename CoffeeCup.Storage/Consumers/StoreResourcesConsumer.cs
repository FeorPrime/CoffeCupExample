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
        await _validator.ValidateAndThrowAsync(context.Message.ResourceDto, context.CancellationToken);
        await _storageService.StoreResourcesAsync(context.Message.ResourceDto, context.CancellationToken);
    }
}

public class StoreResourcesFaultConsumer : IConsumer<Fault<StoreResources>>
{
    private readonly ILogger<StoreResourcesFaultConsumer> _logger;
    public StoreResourcesFaultConsumer(ILogger<StoreResourcesFaultConsumer> logger) { _logger = logger; }
    public Task Consume(ConsumeContext<Fault<StoreResources>> context)
    {
        _logger.LogError("{@Message}", context.Message);
        return Task.CompletedTask;
    }
}