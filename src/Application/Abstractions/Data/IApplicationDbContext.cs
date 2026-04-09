using Domain.Recommendations;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext : IDisposable
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<FoodPlace> FoodPlaces { get; }
    DbSet<Recipe> Recipes { get; }
    DbSet<RecommendationSession> RecommendationSessions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
   
}
