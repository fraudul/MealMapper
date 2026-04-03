using Domain.Recommendations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Recommendations;

internal sealed class FoodPlaceConfiguration : IEntityTypeConfiguration<FoodPlace>
{
    public void Configure(EntityTypeBuilder<FoodPlace> builder)
    {
        builder.ToTable("FoodPlaces");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AveragePricePerPerson).IsRequired().HasPrecision(18, 2);
        builder.Property(x => x.Rating).IsRequired().HasPrecision(3, 2);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.ExternalMapId).HasMaxLength(100);

        // Value Object GeoLocation — сохраняем как owned entity
        builder.OwnsOne(x => x.Location, location =>
        {
            location.Property(l => l.Latitude).IsRequired().HasColumnName("Location_Latitude");
            location.Property(l => l.Longitude).IsRequired().HasColumnName("Location_Longitude");
            location.Property(l => l.City).IsRequired().HasMaxLength(50).HasColumnName("Location_City");
        });
    }
}
