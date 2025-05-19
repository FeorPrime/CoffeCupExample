namespace CoffeeCup.Contracts.Resources;

public static class ResourcesMappers
{
    public static ResourcesDto ToResources(this ResourceEntity? entity) => new()
        {
            Coffee = entity?.Coffee ?? 0f,
            Water = entity?.Water ?? 0f,
        };
}