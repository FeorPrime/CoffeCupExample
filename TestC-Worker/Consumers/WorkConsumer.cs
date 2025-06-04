using System.Globalization;
using MassTransit;
using TestC_contracts;
using TestC_contracts.Entities;

namespace TestC_Worker.Consumers;

public class WorkConsumer : IConsumer<Work>
{
    private readonly TestCContext _ctx;

    public WorkConsumer(TestCContext ctx)
    {
        _ctx = ctx;
    }
    
    public async Task Consume(ConsumeContext<Work> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----------------------------");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("CorrId = " + context.Message.CorrId);
        Console.WriteLine("Payload CorrId = " + context.Message.Payload.CorrId);
        Console.WriteLine("Payload Route = " + context.Message.Payload.Route);
        Console.WriteLine("Where Started = " + context.Message.Payload.WhereStarted);
        Console.WriteLine("Where Finished = " + context.Message.Payload.WhereFinished);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----------------------------");
        Console.ResetColor();

        _ctx.WorkReports.Add(new WorkReport
        {
            CorrId = context.Message.CorrId,
            Route = context.Message.Payload.Route,
            WhereStarted = context.Message.Payload.WhereStarted.ToString(CultureInfo.CurrentCulture),
            WhereFinished = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)//context.Message.Payload.WhereFinished.ToString(CultureInfo.CurrentCulture),
        });
        
        await _ctx.SaveChangesAsync();
    }
}