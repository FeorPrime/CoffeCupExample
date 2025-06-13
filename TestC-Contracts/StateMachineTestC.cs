using MassTransit;
using TestC_contracts.Entities;

namespace TestC_contracts;

public class StateMachineTestC : MassTransitStateMachine<StateMachineTestCState>
{
    public StateMachineTestC()
    {
        InstanceState( x => x.State);

        Event(()=> Starting, 
            x => x.CorrelateById( 
                c => c.Message.CorrId));
        Event(()=> Working,
            x => x.CorrelateById(
                c => c.Message.CorrId));
        Event(() => LastEvent,
            x => x.CorrelateById(
                c => c.Message.CorrId));

        Initially(
            When(Starting)
                .Then(s =>
                {
                    s.Saga.StartStamp = DateTime.UtcNow;
                    s.Saga.Route = s.Message.Route;
                })
                .Publish(ctx => new Work(ctx.Message.CorrId,
                    new WorkPayload
                    {
                        CorrId = ctx.Message.CorrId,
                        Route = ctx.Message.Route,
                        WhenStarted = DateTime.UtcNow
                    })));
        
        During(Working, When());
    }

    public State InProgress { get; } = null!;
    public State Completed { get; } = null!;
    
    public Event<StartMachine> Starting { get; } = null!;
    public Event<Work> Working { get; } = null!;
    public Event<LastEvent> LastEvent { get; } = null!;
}