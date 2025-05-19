using FluentValidation;

namespace CoffeeCup.Contracts.Resources;

public class ResourcesValidator : AbstractValidator<ResourcesDto>
{
    public ResourcesValidator()
    {
        RuleFor(x => x.Coffee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Water).GreaterThanOrEqualTo(0);
    }
}