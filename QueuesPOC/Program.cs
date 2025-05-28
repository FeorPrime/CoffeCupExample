using MassTransit;
using Cocona;
using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations.Schema;
using Foo;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
    options.StopTimeout = TimeSpan.FromMinutes(1);
});

builder.Services.AddMassTransit(x =>
{
    x.SetInMemorySagaRepositoryProvider();

    x.AddConsumer<FileValidationRequestConsumer>();
    x.AddSagaStateMachine<FileStateMachine, FileState, FileStateSagaDefinition>().Endpoint(e =>
    {
        e.Name = "file-state-saga-a";
    }).ExcludeFromConfigureEndpoints();

    x.AddSagaStateMachine<FileStateMachine, FileState, FileStateSagaDefinition>().Endpoint(e =>
    {
        e.Name = "file-state-saga-b";
    }).ExcludeFromConfigureEndpoints();

    x.AddSagaStateMachine<FileStateMachine, FileState, FileStateSagaDefinition>().Endpoint(e =>
    {
        e.Name = "file-state-saga-c";
    }).ExcludeFromConfigureEndpoints();


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("password");
        });

        cfg.Send<FileAccepted>(x =>
        {
            // use customerType for the routing key
            x.UseRoutingKeyFormatter(context => context.Message.StockExchange);

            x.UseCorrelationId(m => m.CorrelationId);
        });

        cfg.ReceiveEndpoint("file-state-saga-a", (IRabbitMqReceiveEndpointConfigurator x) =>
        {
            x.Bind<FileAccepted>(s =>
            {
                s.RoutingKey = "a";
                s.ExchangeType = ExchangeType.Direct;
            });
            x.StateMachineSaga<FileState>(context);
        });

        cfg.ReceiveEndpoint("file-state-saga-b", (IRabbitMqReceiveEndpointConfigurator x) =>
        {
            x.Bind<FileAccepted>(s =>
            {
                s.RoutingKey = "b";
                s.ExchangeType = ExchangeType.Direct;
            });
            x.StateMachineSaga<FileState>(context);
        });

        cfg.ReceiveEndpoint("file-state-saga-c", (IRabbitMqReceiveEndpointConfigurator x) =>
        {
            x.Bind<FileAccepted>(s =>
            {
                s.RoutingKey = "c";
                s.ExchangeType = ExchangeType.Direct;
            });
            x.StateMachineSaga<FileState>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

app.AddCommand(async () =>
{
    for (; ; )
    {
        await Task.Yield();
    }
});

await app.RunAsync();

namespace Foo
{

    [Table(nameof(FileState))]
    public class FileState :
        SagaStateMachineInstance, ISagaVersion
    {
        public string CurrentState { get; set; }
        public string StockExchange { get; set; }
        public int Version { get; set; }
        public Guid CorrelationId { get; set; }

    }


    public record FileAccepted(Guid CorrelationId) : CorrelatedBy<Guid>
    {
        public string FullPath { get; init; }
        public string StockExchange { get; init; }
    }

    public sealed class FileStateSagaDefinition : SagaDefinition<FileState>
    {
        private const int ConcurrencyLimit = 64; // this can go up, depending upon the database capacity

        public FileStateSagaDefinition()
        {
            // specify the message limit at the endpoint level, which influences
            // the endpoint prefetch count, if supported.
            Endpoint(e =>
            {
                e.PrefetchCount = 1024;
                e.ConcurrentMessageLimit = ConcurrencyLimit;
            });
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<FileState> sagaConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Exponential(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3)));
            endpointConfigurator.UseScheduledRedelivery(r => r.Exponential(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3)));
            endpointConfigurator.UseInMemoryOutbox(context);
            var partition = endpointConfigurator.CreatePartitioner(ConcurrencyLimit);

            sagaConfigurator.Message<FileAccepted>(x =>
            {
                x.UsePartitioner(partition, m => m.Message.CorrelationId);
            });
            sagaConfigurator.Message<FileValidated>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
        }
    }

    public partial class FileStateMachine :
        MassTransitStateMachine<FileState>
    {
        public Event<FileAccepted> FileAcceptedEvent { get; set; } = null!;
        public Request<FileState, FileValidationRequest, FileValidated> FileValidationRequest { get; set; } = null!;
        public State WaitForValidation { get; set; } = null!;

        public FileStateMachine()
        {
            InstanceState(m => m.CurrentState);
            Request(() => FileValidationRequest, r =>
            {
                r.Completed = m => m.OnMissingInstance(i => i.Discard());
                r.Faulted = m => m.OnMissingInstance(i => i.Discard());
                r.TimeoutExpired = m => m.OnMissingInstance(i => i.Discard());
            });
            Event(() => FileAcceptedEvent);

            // Handle file accept
            Initially(
                When(FileAcceptedEvent)
                    .Then(context =>
                    {
                        context.Saga.StockExchange = context.Message.StockExchange;
                    })
                    .Request(FileValidationRequest, context => new(context.Message.CorrelationId))
                    .TransitionTo(FileValidationRequest.Pending)
            );

            During(FileValidationRequest.Pending,
                When(FileValidationRequest.Completed)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }

    public record FileValidated(Guid CorrelationId) : CorrelatedBy<Guid>;
    public record FileValidationRequest(Guid CorrelationId) : CorrelatedBy<Guid>;

    public class FileValidationRequestConsumer : IConsumer<FileValidationRequest>
    {
        public async Task Consume(ConsumeContext<FileValidationRequest> context)
        {
            await context.RespondAsync(new FileValidated(context.Message.CorrelationId));
        }

    }
}