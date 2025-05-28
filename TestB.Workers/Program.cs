using MassTransit;
using RabbitMQ.Client;
using TestB.Contracts;
using TestBWorkers.Consumers;

var builder = WebApplication.CreateBuilder(args);

var appKey = Environment.GetEnvironmentVariable("ROUTING") ?? throw new InvalidOperationException("Missing environment variable ROUTING");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SimpleConsumer>();
        // .Endpoint(e =>
        // {
        //     //e.Name = $"event-{appKey}";
        //     e.InstanceId = appKey;
        // });//.Endpoint(e => e.Name = $"event-{appKey.ToLower()}");
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.AddMTHost();
        
        // cfg.Message<TheEvent>(m =>
        // {
        //     m.SetEntityName($"event-routed-{appKey.ToLower()}");
        // });
        //
        cfg.ReceiveEndpoint(LoadBalancer.GetQueueByAppKey(appKey.ToUpper()), e =>
        {
            e.Bind<TheEvent>(b =>
            {
                b.RoutingKey = appKey;
                //b.ExchangeType = ExchangeType.Direct;
            });
            e.ConfigureConsumer<SimpleConsumer>(ctx);
            //e.ExchangeType = ExchangeType.Direct;
        });
        //
        //cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

Helpers.PrintPretty($"Started with key: {appKey}, bound to: queue:{LoadBalancer.GetQueueByAppKey(appKey.ToUpper())}");

app.Run();