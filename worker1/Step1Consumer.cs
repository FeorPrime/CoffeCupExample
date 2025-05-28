using contracts;
using contracts.Entities;
using MassTransit;

namespace worker1;

public class Step1Consumer : IConsumer<Step1Started>
{
    private readonly ExpContext _ctx;

    public Step1Consumer(ExpContext ctx) { _ctx = ctx; }

    public async Task Consume(ConsumeContext<Step1Started> context)
    {
        STMHelper.PrintPretty($"Received message: {context.Message.Payload.TextPayLoad}");

        _ctx.WorkReports.Add(new WorkReport
        {
            Id = Guid.NewGuid(),
            Processed = DateTime.UtcNow,
            TextPayLoad = context.Message.Payload.TextPayLoad,
            WorkerId = WorkerMeta.WorkerId
        });
        
        await _ctx.SaveChangesAsync();

        await context.Publish(new Step1Done(context.Message.CorrelationId));
    }
}