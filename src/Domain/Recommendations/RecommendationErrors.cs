using SharedKernel;

namespace Domain.Recommendations;

public static class RecommendationErrors
{
    public static Error InvalidBudget =>
        Error.Failure("Recommendation.InvalidBudget", "Некорректный диапазон бюджета");

    public static Error InvalidLocation =>
        Error.Failure("Recommendation.InvalidLocation", "Координаты вне Минска");

    public static Error NoRecommendationsFound =>
        Error.NotFound("Recommendation.NoRecommendationsFound", "Не удалось найти подходящие варианты");
}
