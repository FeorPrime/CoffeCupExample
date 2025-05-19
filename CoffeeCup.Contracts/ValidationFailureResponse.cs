namespace CoffeeCup.Contracts;

public record ValidationFailureResponse
{
    public IEnumerable<ValidationResponse> Errors { get; init; }
}

public record ValidationResponse
{
    public string PropertyName { get; init; }
    public string Message { get; init; }
}