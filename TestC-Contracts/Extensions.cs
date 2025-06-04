using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TestC_contracts.Entities;

namespace TestC_contracts;

public static class Extensions
{
    private static string GetInstanceName()
    {
        return Environment.GetEnvironmentVariable("ROUTE_NAME") ?? throw new InvalidOperationException("ROUTE_NAME environment variable is missing.");
    }
    
    public static IServiceCollection AddDb(this IServiceCollection services)
    {
        services.AddDbContext<TestCContext>();
        return services;
    }

    private static void DefaultHost(this IRabbitMqBusFactoryConfigurator configurator)
    {
        configurator.Host("rabbitmq://localhost", rmqcfg =>
        {
            rmqcfg.Username("admin");
            rmqcfg.Password("password");
        });
    }
    
    public static void AddTestCConnection(this IServiceCollection service)
    {
        service.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetEntryAssembly());
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.DefaultHost();
                
                cfg.Send<StartMachine>(x =>
                {
                    x.UseRoutingKeyFormatter(ctx => ctx.Message.Route);
                    x.UseCorrelationId(m => m.CorrId);
                });
                
                //cfg.ConfigureEndpoints(context);
            });

            x.AddInMemoryInboxOutbox();
        });
    }

    public static void AddStateMachine(this IServiceCollection service)
    {
        service.AddMassTransit(x =>
        {
            x.AddSagaStateMachine<StateMachineTestC, StateMachineTestCState>()
                .EntityFrameworkRepository(cfg =>
                {
                    cfg.UsePostgres();
                    cfg.ExistingDbContext<TestCContext>();
                })
                .Endpoint(e =>
                {
                    e.Name = "TestCStateMachine";
                    e.InstanceId = GetInstanceName();
                });
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.DefaultHost();
                //cfg.ConfigureEndpoints(context);
            });
        });
    }
}