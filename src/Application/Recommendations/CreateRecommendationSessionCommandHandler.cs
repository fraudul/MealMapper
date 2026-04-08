using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Recommendations;
using MediatR;
using SharedKernel;

namespace Application.Recommendations;

internal sealed class CreateRecommendationSessionCommandHandler 
    : ICommandHandler<CreateRecommendationSessionCommand, Result<RecommendationSessionResponse>>
{
    private readonly IRecommendationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateRecommendationSessionCommandHandler(IRecommendationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result<RecommendationSessionResponse>>> Handle(
        CreateRecommendationSessionCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Создаём Value Objects
        var userLocation = new GeoLocation(request.Latitude, request.Longitude);
        userLocation.Validate();

        var budget = new BudgetRange(request.BudgetMin, request.BudgetMax);
        budget.Validate();

        var preferences = new UserPreferences(
            request.ActionType,
            request.IsFastFoodAllowed,
            request.DistancePref,
            request.CuisinePreference,
            request.OnlyNearby);

        // 2. Создаём Aggregate
        var session = new RecommendationSession(userLocation, budget, preferences);

        // 3. Получаем рекомендации из репозитория
        List<RecommendationItem> recommendations;

        if (preferences.ActionType == FoodActionType.CookAtHome)
        {
            List<Recipe> matchingRecipes = await _repository.FindMatchingRecipesAsync(
                budget, preferences, 10, cancellationToken);

            recommendations = matchingRecipes.Select(r => new RecommendationItem(
                r.Id,
                r.Name,
                $"{r.EstimatedCost} BYN • {r.CookingTimeMinutes} мин",
                "Recipe",
                0.85, // временный скоринг
                "Подходит под ваш бюджет и время"
            )).ToList();
        }
        else
        {
            List<FoodPlace> matchingPlaces = await _repository.FindMatchingFoodPlacesAsync(
                userLocation, budget, preferences, 10, cancellationToken);

            recommendations = matchingPlaces.Select(p => new RecommendationItem(
                p.Id,
                p.Name,
                $"{p.AveragePricePerPerson} BYN • {Math.Round(p.Location.DistanceTo(userLocation)/1000, 1)} км",
                "FoodPlace",
                p.Rating / 5.0,
                p.IsOpenNow ? "Открыто сейчас" : "Закрыто"
            )).ToList();
        }

        // 4. Добавляем в Aggregate и сохраняем
        session.AddRecommendations(recommendations);

        await _repository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Возвращаем ответ
        var response = new RecommendationSessionResponse(session.Id, recommendations.Take(3).ToList());

        return Result.Success(response);
    }
}
