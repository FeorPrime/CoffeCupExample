using MassTransit;
using TestB.Contracts;

namespace TestBWorkers.Consumers;

public class SimpleConsumer: IConsumer<TheEvent>
{
    public Task Consume(ConsumeContext<TheEvent> context)
    {
        Helpers.PrintPretty($"Consuming {context.Message.CorrId} for {context.Message.Target}");
        return Task.CompletedTask;
    }
}