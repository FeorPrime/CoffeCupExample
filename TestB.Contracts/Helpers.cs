using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace TestB.Contracts;

public static class Helpers
{
    public static void PrintPretty(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---------------------------");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---------------------------");
        Console.ResetColor();
    }
}

public class LoadBalancer
{
    private static bool State = false;

    public string GetRouteKeyByRoundRobin()
    {
        if (State)
        {
            State = !State;
            return "A";
        }
        State = !State;
        return "B";
    }

    public string GetQueueByRoundRobin()
    {
        if (State)
        {
            State = !State;
            return QueueHelpers.SimpleQueueBase + "A";
        }
        State = !State;
        return QueueHelpers.SimpleQueueBase + "B";
    }

    public static string GetQueueByAppKey(string appKey) =>
        appKey switch
        {
            "A" => QueueHelpers.SimpleQueueBase + "A",
            "B" => QueueHelpers.SimpleQueueBase + "B",
            _ => throw new NotImplementedException()
        };
}

public static class LoadBalancerHelper
{
    public static void AddLoadBalancer(this IServiceCollection services) => services.AddSingleton<LoadBalancer>();
}

public static class TransportHelpers
{
    public static void AddMt(this IServiceCollection services)
    {
        var assembly = Assembly.GetEntryAssembly();
        services.AddMassTransit(x =>
        {
            x.AddConsumers(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", rmqcfg => { rmqcfg.Username("admin"); rmqcfg.Password("password"); });
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static void AddMTHost(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("admin");
            h.Password("password");
        });
    }
}

public static class QueueHelpers
{
    public static string SimpleQueueBase = "Simple";
}