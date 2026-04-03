using Domain.Recommendations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Recommendations;

internal sealed class RecommendationSessionConfiguration : IEntityTypeConfiguration<RecommendationSession>
{
    public void Configure(EntityTypeBuilder<RecommendationSession> builder)
    {
        builder.ToTable("RecommendationSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.OwnsOne(x => x.Budget, budget =>
        {
            budget.Property(b => b.Min).IsRequired().HasColumnName("Budget_Min").HasPrecision(18, 2);
            budget.Property(b => b.Max).IsRequired().HasColumnName("Budget_Max").HasPrecision(18, 2);
        });

        builder.OwnsOne(x => x.Preferences, prefs =>
        {
            prefs.Property(p => p.ActionType).IsRequired().HasColumnName("Preferences_ActionType");
            prefs.Property(p => p.IsFastFoodAllowed).IsRequired().HasColumnName("Preferences_IsFastFoodAllowed");
            prefs.Property(p => p.DistancePref).IsRequired().HasColumnName("Preferences_DistancePref");
            prefs.Property(p => p.CuisinePreference).HasMaxLength(100).HasColumnName("Preferences_CuisinePreference");
            prefs.Property(p => p.OnlyNearby).IsRequired().HasColumnName("Preferences_OnlyNearby");
        });

        // Коллекция рекомендаций пока не сохраняем в БД (будет в отдельной таблице позже)
        builder.Ignore(x => x.Items);
    }
}
