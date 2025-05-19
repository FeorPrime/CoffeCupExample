namespace CoffeeCup.Contracts.Resources;

public record StoreResources(Guid CorrelationId, ResourcesDto ResourceDto);
public record TakeResources(Guid CorrelationId, ResourcesDto ResourceDto, bool? Result = null);
public record CheckResources(Guid CorrelationId);
public record ResourcesReport(Guid CorrelationId, ResourcesDto ResourceDto);
public record PruneResources(Guid CorrelationId);