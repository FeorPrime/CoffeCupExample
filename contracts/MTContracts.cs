using System.Reflection;
using contracts.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace contracts;

public record StartMachine(Guid RequestId, string StockExchange);
public record Step1Started(Guid CorrelationId, SimpleContract Payload);
public record Step1Done(Guid CorrelationId);
public record Step2Started(Guid CorrelationId, SimpleContract Payload);
public record Step2Done(Guid CorrelationId);
//public record MachineIsFinished(Guid CorrelationId);


public static class MtExtensions
{
    public static void AddTransport(this IServiceCollection services, List<IConsumer> consumers = null)
    {
        var assembly = Assembly.GetEntryAssembly();
        services.AddMassTransit(x =>
        {
            // x.AddEntityFrameworkOutbox<ExpContext>(o =>
            // {
            //     o.QueryDelay = TimeSpan.FromSeconds(1);
            //     o.UsePostgres();
            //     o.UseBusOutbox();
            // });
            
            x.AddConsumers(assembly);
            // x.AddConfigureEndpointsCallback((context, name, cfg) =>
            // {
            //     cfg.UseEntityFrameworkOutbox<ExpContext>(context);
            // });
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", rmqcfg => { rmqcfg.Username("admin"); rmqcfg.Password("password"); });
                cfg.Send<StartMachine>(x =>
                {
                    x.UseRoutingKeyFormatter(ctx => ctx.Message.StockExchange);
                    x.UseCorrelationId(m => m.RequestId);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}