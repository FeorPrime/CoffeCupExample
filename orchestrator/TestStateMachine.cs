using contracts;
using contracts.Contracts;
using contracts.Entities;
using MassTransit;
using worker1;

namespace orchestrator;

public class TestStateMachine: MassTransitStateMachine<TestStateMachineState>
{
    public TestStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => MachineStarted, x => x.CorrelateById(c => c.Message.RequestId));
        Event(() => Step1Started, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => Step1Done, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => Step2Started, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => Step2Done, x => x.CorrelateById(c => c.Message.CorrelationId));
       //Event(() => MachineIsFinished, x => x.CorrelateById(c => c.Message.CorrelationId));
        
        Initially(
            When(MachineStarted)
                .Then(ctx =>
                {
                    ctx.Saga.CorrelationId = ctx.Message.RequestId;
                    ctx.Saga.StartStamp = DateTime.UtcNow;
                    ctx.Saga.MachineId = WorkerMeta.WorkerId;
                    STMHelper.PrintSTMInfo(ctx.Saga);
                })
                .Publish( ctx => new Step1Started(ctx.Saga.CorrelationId, new SimpleContract
                {
                    CorrelationId = ctx.Message.RequestId,
                    TextPayLoad = $"saga started in {WorkerMeta.WorkerId}",
                    Worker = WorkerMeta.WorkerId
                }))
                .TransitionTo(Step1));
        
        During(Step1, When(Step1Started)
            .Then(ctx =>
            {
                Console.WriteLine("Step1Started event was produced");
            }));
        
        During(Step1,
            When(Step1Done)
                .Publish( ctx => new Step2Started(ctx.Saga.CorrelationId, new SimpleContract
                {
                    CorrelationId = ctx.Message.CorrelationId,
                    TextPayLoad = $"saga proceeded in {WorkerMeta.WorkerId}",
                    Worker = WorkerMeta.WorkerId
                }))
                .Then(ctx =>
                {
                    ctx.Saga.Step1Done = true;
                    STMHelper.PrintSTMInfo(ctx.Saga);
                })
                .TransitionTo(Step2));
        
        During(Step2, When(Step2Started)
            .Then(ctx =>
            {
                Console.WriteLine("Step2Started event was produced");
            }));
        
        During(Step2,
            When(Step2Done)
                //.Publish(ctx => new MachineIsFinished(ctx.Saga.CorrelationId))
                .Then(ctx =>
                {
                    ctx.Saga.Step2Done = true;
                    ctx.Saga.EndStamp = DateTime.UtcNow;
                    STMHelper.PrintSTMInfo(ctx.Saga);
                })
                .Finalize());
    }

    public State Step1 { get; } = null!;
    public State Step2 { get; } = null!;
    
    public Event<StartMachine> MachineStarted { get; } = null!;
    public Event<Step1Started> Step1Started { get; } = null!;
    public Event<Step1Done> Step1Done { get; } = null!;
    public Event<Step2Started> Step2Started { get; } = null!;
    public Event<Step2Done> Step2Done { get; } = null!;
    //public Event<MachineIsFinished> MachineIsFinished { get; } = null!;
}