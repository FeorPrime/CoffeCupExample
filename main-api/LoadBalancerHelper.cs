namespace api;

public class LoadBalancerHelper
{
    public const string StockExchangePrefix = "machine-events-";

    private static bool State = false;
    
    public string GetExchange()
    {
        if (State)
        {
            State = !State;
            return StockExchangePrefix + "alpha";
        }

        State =!State;
        return StockExchangePrefix + "beta";
    }
}

public static class LoadBalancerHelperExtensions
{
    public static void AddLoadBalancerHelper(this IServiceCollection services)
    {
        services.AddSingleton<LoadBalancerHelper>();
    }
}