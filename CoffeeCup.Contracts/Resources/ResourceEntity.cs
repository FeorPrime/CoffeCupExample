using System.ComponentModel.DataAnnotations;

namespace CoffeeCup.Contracts.Resources;

public class ResourceEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public float Water { get; set; } = 100;
    public float Coffee { get; set; } = 100;
}