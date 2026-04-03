using Application.Abstractions.Repositories;
using Domain.Recommendations;
using MediatR;
using SharedKernel;

namespace Application.Recommendations;

public sealed record CreateRecommendationSessionCommand(
    double Latitude,
    double Longitude,
    decimal BudgetMin,
    decimal BudgetMax,
    FoodActionType ActionType,
    bool IsFastFoodAllowed,
    DistancePreference DistancePref,
    string? CuisinePreference = null,
    bool OnlyNearby = false)
    : IRequest<Result<RecommendationSessionResponse>>;

public sealed record RecommendationSessionResponse(
    Guid SessionId,
    List<RecommendationItem> Recommendations);
