namespace contracts.Contracts;

public record SimpleContract
{
    public Guid CorrelationId { get; init; }
    public string TextPayLoad { get; set; }
    public Guid Worker { get; set; }
}