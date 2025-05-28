using System.ComponentModel.DataAnnotations;

namespace contracts.Entities;

public class WorkReport
{
    [Key]
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string TextPayLoad { get; set; }
    public DateTime Processed { get; set; }
}