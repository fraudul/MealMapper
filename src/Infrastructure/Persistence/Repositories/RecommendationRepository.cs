using Application.Abstractions.Repositories;
using Domain.Recommendations;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class RecommendationRepository : IRecommendationRepository
{
    private readonly ApplicationDbContext _context;

    public RecommendationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RecommendationSession session, CancellationToken cancellationToken = default)
    {
        await _context.RecommendationSessions.AddAsync(session, cancellationToken);
    }

    /*
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    */
    public Task<RecommendationSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.RecommendationSessions
            .AsNoTracking()                    // важно для read-only запросов
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<FoodPlace>> FindMatchingFoodPlacesAsync(
        GeoLocation userLocation,
        BudgetRange budget,
        UserPreferences preferences,
        int limit,
        CancellationToken cancellationToken = default)
    {
        // Здесь пока простой запрос. Позже добавим Domain Service с более умным скорингом + интеграцией карт
        return await _context.FoodPlaces
            .AsNoTracking()
            .Where(place => place.MatchesPreferences(preferences, userLocation, budget))
            .OrderByDescending(place => place.Rating)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Recipe>> FindMatchingRecipesAsync(
        BudgetRange budget,
        UserPreferences preferences,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Where(recipe => recipe.MatchesBudget(budget))
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}
