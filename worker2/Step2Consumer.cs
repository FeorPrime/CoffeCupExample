using contracts;
using contracts.Entities;
using MassTransit;
using worker1;

namespace worker2;

public class Step2Consumer : IConsumer<Step2Started>
{
    private readonly ExpContext _ctx;

    public Step2Consumer(ExpContext ctx) { _ctx = ctx; }

    public async Task Consume(ConsumeContext<Step2Started> context)
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

        await context.Publish(new Step2Done(context.Message.CorrelationId));
    }
}