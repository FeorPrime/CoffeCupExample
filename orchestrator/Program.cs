using System.Data;
using contracts;
using contracts.Entities;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Middleware;
using Microsoft.EntityFrameworkCore;
using orchestrator;
using RabbitMQ.Client;
using worker1;

var builder = WebApplication.CreateBuilder(args);

var orchestratorQueue = Environment.GetEnvironmentVariable("ORCHESTRATOR_QUEUE") ?? "alpha";

STMHelper.PrintPretty(orchestratorQueue);

builder.Services.AddDb();

builder.Services.AddMassTransit(x =>
{
    x.AddInMemoryInboxOutbox();
    
    x.AddEntityFrameworkOutbox<ExpContext>(o =>
    {
        o.LockStatementProvider = new PostgresLockStatementProvider();
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(2);
        o.IsolationLevel = IsolationLevel.Serializable;
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddSagaStateMachine<TestStateMachine, TestStateMachineState, RegistrationStateDefinition>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            r.LockStatementProvider = new PostgresLockStatementProvider();
            r.IsolationLevel = IsolationLevel.Serializable;
            //r.AddDbContext<DbContext, ExpContext>();
            r.ExistingDbContext<ExpContext>();
            r.UsePostgres();
        });
    //.Endpoint(e=>
    //    {
      //      e.Name = $"machine-events-{orchestratorQueue}";
       // });
        //.ExcludeFromConfigureEndpoints();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", rmqcfg => { rmqcfg.Username("admin"); rmqcfg.Password("password"); });
        
        cfg.ReceiveEndpoint($"machine-events-{orchestratorQueue}", e =>
        {
            // const int concurrencyLimit = 10;
            // e.PrefetchCount = concurrencyLimit;
            e.Bind<StartMachine>(s =>
            {
                s.RoutingKey = $"machine-events-{orchestratorQueue}";
                s.ExchangeType = ExchangeType.Direct;
            });
            
            e.UseMessageRetry(r => r.Interval(5,1000));
            //e.UseInMemoryOutbox(context);
            
            // e.ConfigureSaga<TestStateMachineState>(context, s =>
            // {
            //     var partition = s.CreatePartitioner(concurrencyLimit);
            //     s.Message<StartMachine>(xx => xx.UsePartitioner(partition, m => m.Message.RequestId));
            //     s.Message<Step1Started>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
            //     s.Message<Step2Started>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
            //     s.Message<Step1Done>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
            //     s.Message<Step2Done>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
            // });
            //e.UsePartitioner<TestStateMachineState>(new Partitioner(16, new Murmur3UnsafeHashGenerator()), consumeContext => consumeContext.Message.CorrelationId );
            //e.UseEntityFrameworkOutbox<ExpContext>(context);
        });
        
        // cfg.ReceiveEndpoint("machine-events_error", config =>
        // {
        //      config.ConfigureError( m =>
        //      {
        //                            
        //      });
        // });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run();

public class RegistrationStateDefinition :
    SagaDefinition<TestStateMachineState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<TestStateMachineState> consumerConfigurator, IRegistrationContext context)
    {
       // endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));
        
        endpointConfigurator.UseEntityFrameworkOutbox<ExpContext>(context);
        // endpointConfigurator.ConfigureSaga<TestStateMachineState>(s =>
        // {
        //     var partition = s.CreatePartitioner(10);
        //     s.Message<StartMachine>(xx => xx.UsePartitioner(partition, m => m.Message.RequestId));
        //     s.Message<Step1Started>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
        //     s.Message<Step2Started>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
        //     s.Message<Step1Done>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
        //     s.Message<Step2Done>(xx => xx.UsePartitioner(partition, m => m.Message.CorrelationId));
        // });
        // endpointConfigurator.UseInMemoryInboxOutbox(context);//.UseInMemoryOutbox();
    }
}