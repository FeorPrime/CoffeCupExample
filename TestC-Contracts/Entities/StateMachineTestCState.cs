using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace TestC_contracts.Entities;

public class StateMachineTestCState : SagaStateMachineInstance
{
    [Key]
    public Guid CorrelationId { get; set; }
    
    public string State { get; set; }

    public string Route { get; set; }
    public string WhereStarted { get; set; }
    public string WhereFinished { get; set; }
    
    public string Work { get; set; }
    
    public DateTime? StartStamp { get; set; }
    public DateTime? EndStamp { get; set; }
}