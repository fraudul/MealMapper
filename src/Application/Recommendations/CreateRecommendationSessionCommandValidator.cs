using Domain.Recommendations;
using FluentValidation;

namespace Application.Recommendations;

public sealed class CreateRecommendationSessionCommandValidator : AbstractValidator<CreateRecommendationSessionCommand>
{
    public CreateRecommendationSessionCommandValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(53.8, 54.0)
            .WithMessage("Координаты должны быть в пределах Минска");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(27.4, 27.7)
            .WithMessage("Координаты должны быть в пределах Минска");

        RuleFor(x => x.BudgetMin)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.BudgetMax)
            .GreaterThanOrEqualTo(x => x.BudgetMin)
            .WithMessage("Максимальный бюджет должен быть больше минимального");

        RuleFor(x => x.ActionType)
            .IsInEnum();
    }
}
