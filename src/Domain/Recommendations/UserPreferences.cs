namespace Domain.Recommendations;

public sealed record UserPreferences(
    FoodActionType ActionType,
    bool IsFastFoodAllowed,
    DistancePreference DistancePref,
    string? CuisinePreference = null,
    bool OnlyNearby = false);

public enum FoodActionType { Buy, Order, CookAtHome }
public enum DistancePreference { Near, Medium, Far }
