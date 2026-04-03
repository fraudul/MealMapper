using SharedKernel;

namespace Domain.Recommendations;

public class FoodPlace : Entity
{
    public Guid Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public GeoLocation Location { get; private set; } = null!;
    public string Category { get; private set; } = string.Empty;
    public decimal AveragePricePerPerson { get; private set; }
    public double Rating { get; private set; }
    public string? Address { get; private set; }
    public bool IsOpenNow { get; private set; }
    public string? ExternalMapId { get; private set; }

    private FoodPlace() { } // для EF Core

    public FoodPlace(string name, GeoLocation location, string category,
        decimal avgPrice, double rating, string? address = null, string? externalMapId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Location = location;
        Category = category;
        AveragePricePerPerson = avgPrice;
        Rating = rating;
        Address = address;
        ExternalMapId = externalMapId;
        IsOpenNow = true;
    }

    public bool MatchesPreferences(UserPreferences prefs, GeoLocation userLocation, BudgetRange budget)
    {
        if (!budget.Fits(AveragePricePerPerson))
        {
            return false;
        }

        if (!prefs.IsFastFoodAllowed && Category.Contains("Фастфуд", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        double distance = userLocation.DistanceTo(Location);
        return prefs.DistancePref switch
        {
            DistancePreference.Near => distance <= 1500,
            DistancePreference.Medium => distance <= 5000,
            _ => true
        };
    }
}
