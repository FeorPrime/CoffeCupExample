namespace TestC_contracts;

public record StartMachine(Guid CorrId, string Route);

public record Work(Guid CorrId, WorkPayload Payload);

public record LastEvent(Guid CorrId, WorkPayload Payload);

public class WorkPayload
{
    public Guid CorrId { get; set; }
    
    public string Route { get; set; }
    public DateTime WhereStarted { get; set; }
    public DateTime WhereFinished { get; set; }
}