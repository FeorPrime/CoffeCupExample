using FluentValidation;

namespace CoffeeCup.Contracts.Resources;

public class ResourcesValidator : AbstractValidator<ResourcesDto>
{
    public ResourcesValidator()
    {
        RuleFor(x => x.Coffee).GreaterThan(0);
        RuleFor(x => x.Water).GreaterThan(0);
    }
}