using MassTransit;
using TestC_contracts;

namespace TestC_Api.Consumers;

public class FinalConsumer : IConsumer<LastEvent>
{
    public Task Consume(ConsumeContext<LastEvent> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----------------------------");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("CorrId = " + context.Message.CorrId);
        Console.WriteLine("Payload CorrId = " + context.Message.Payload.CorrId);
        Console.WriteLine("Payload Route = " + context.Message.Payload.Route);
        Console.WriteLine("Where Started = " + context.Message.Payload.WhenStarted);
        Console.WriteLine("Where Finished = " + context.Message.Payload.WhenFinished);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----------------------------");
        Console.ResetColor();
        
        return Task.CompletedTask;
    }
}