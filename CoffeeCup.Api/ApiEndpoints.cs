namespace CoffeeCup.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "/api";

    public static class Storage
    {
        private const string Base = ApiBase + "/storage";
        
        public const string StoreResources = $"{Base}/store-resources";
        public const string CheckResources = $"{Base}/check-resources";
        public const string TakeResources = $"{Base}/take-resources";
        
        public const string PruneStorage = $"{Base}/prune-storage";
    }
}