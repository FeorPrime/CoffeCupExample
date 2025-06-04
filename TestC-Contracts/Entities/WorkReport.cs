using System.ComponentModel.DataAnnotations;

namespace TestC_contracts.Entities;

public class WorkReport
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CorrId { get; set; }
    
    public string Work {get; set;}
    
    public string Route { get; set; }
    public string WhereStarted { get; set; }
    public string WhereFinished { get; set; }
}