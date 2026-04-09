//using Domain.Recommendations;
//using Domain.Todos;      // уже есть в шаблоне
//using Domain.Users;      // уже есть в шаблоне
//using Microsoft.EntityFrameworkCore;
//
//namespace Infrastructure.Persistence;
//
//public class ApplicationDbContext : DbContext
//{
//    public DbSet<User> Users { get; set; } = null!;
//    public DbSet<TodoItem> Todos { get; set; } = null!;
//
//    // Наши новые сущности
//    public DbSet<FoodPlace> FoodPlaces { get; set; } = null!;
//    public DbSet<Recipe> Recipes { get; set; } = null!;
//    public DbSet<RecommendationSession> RecommendationSessions { get; set; } = null!;
//
//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//        : base(options)
//    {
//    }
//
//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        // Автоматически подхватит все наши *Configuration.cs из папки Configurations/Recomendations
//        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
//
//        base.OnModelCreating(modelBuilder);
//    }
//}
