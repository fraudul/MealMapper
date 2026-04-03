using SharedKernel;

namespace Domain.Recommendations;

public class RecommendationSession : Entity
{
    public Guid Id { get; set; }
    public GeoLocation UserLocation { get; private set; } = null!;
    public BudgetRange Budget { get; private set; } = null!;
    public UserPreferences Preferences { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private readonly List<RecommendationItem> _items = new();
    public IReadOnlyCollection<RecommendationItem> Items => _items.AsReadOnly();

    private RecommendationSession() { }

    public RecommendationSession(GeoLocation userLocation, BudgetRange budget, UserPreferences preferences)
    {
        Id = Guid.NewGuid();
        UserLocation = userLocation;
        Budget = budget;
        Preferences = preferences;
        CreatedAt = DateTime.UtcNow;

        // Пример Domain Event (будем использовать позже)
        Raise(new RecommendationSessionCreatedDomainEvent(Id));
    }

    public void AddRecommendations(IEnumerable<RecommendationItem> items)
    {
        _items.Clear();                    // инвариант: всегда максимум 3
        _items.AddRange(items.Take(3));
    }
}

public sealed record RecommendationItem(
    Guid Id,
    string Title,
    string Subtitle,
    string Type,           // "FoodPlace" или "Recipe"
    double Score,
    string Reason);
