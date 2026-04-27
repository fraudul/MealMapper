using Application.Abstractions.Messaging;
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
    //: IRequest<Result<RecommendationSessionResponse>>;
    : ICommand<Result<RecommendationSessionResponse>>;
public sealed record RecommendationSessionResponse(
    Guid SessionId,
    List<RecommendationItem> Recommendations);
/*public sealed record RecommendationItem(
    Guid Id,
    string Name,
    string Description,
    string Type,           // "FoodPlace" или "Recipe"
    double Score,          // от 0.0 до 1.0
    string Reason);*/
