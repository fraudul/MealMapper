using SharedKernel;

namespace Domain.Recommendations;

public class Recipe : Entity
{
    public Guid Id { get; set; }
    public string Name { get; private set; } = string.Empty;
    public decimal EstimatedCost { get; private set; }
    public int CookingTimeMinutes { get; private set; }
    public List<string> Ingredients { get; private set; } = new();
    public List<string> Steps { get; private set; } = new();
    public string? Cuisine { get; private set; }

    private Recipe() { }

    public Recipe(string name, decimal cost, int time, List<string> ingredients,
        List<string> steps, string? cuisine = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        EstimatedCost = cost;
        CookingTimeMinutes = time;
        Ingredients = ingredients;
        Steps = steps;
        Cuisine = cuisine;
    }

    public bool MatchesBudget(BudgetRange budget) => budget.Fits(EstimatedCost);
}
