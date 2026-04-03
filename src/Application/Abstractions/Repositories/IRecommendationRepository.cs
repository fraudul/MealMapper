using Domain.Recommendations;

namespace Application.Abstractions.Repositories;

public interface IRecommendationRepository
{
    // Команды (write)
    Task AddAsync(RecommendationSession session, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    // Запросы (read)
    Task<RecommendationSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Поиск подходящих мест (для рекомендаций)
    Task<List<FoodPlace>> FindMatchingFoodPlacesAsync(
        GeoLocation userLocation,
        BudgetRange budget,
        UserPreferences preferences,
        int limit,
        CancellationToken cancellationToken = default);

    // Поиск рецептов
    Task<List<Recipe>> FindMatchingRecipesAsync(
        BudgetRange budget,
        UserPreferences preferences,
        int limit,
        CancellationToken cancellationToken = default);
}
