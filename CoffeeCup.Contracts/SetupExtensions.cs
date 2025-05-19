using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCup.Contracts;

public static class SetupExtensions
{
    public static IServiceCollection AddTransport(this IServiceCollection services, List<IConsumer> consumers = null)
    {
        var assembly = Assembly.GetEntryAssembly();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumers(assembly);
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", rmqcfg =>
                {
                    rmqcfg.Username("admin");
                    rmqcfg.Password("password");
                });
                cfg.ConfigureEndpoints(context);
                
            });
        });
        
        return services;
    }
    
    public static IServiceCollection AddDb(this IServiceCollection services)
    {
        services.AddDbContext<CoffeeCupStorageContext>();
        return services;
    }
}