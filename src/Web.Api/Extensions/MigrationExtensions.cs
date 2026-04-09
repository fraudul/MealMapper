using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Extensions;


public static class MigrationExtensions
{
    /*
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
    */
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
    
        try 
        {
            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
            // Используем асинхронную версию
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Если база еще не проснулась, мы увидим ошибку в логах, 
            // но приложение не упадет с Code 139
            ILogger<ApplicationDbContext> logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while applying migrations.");
        }
    }
}




