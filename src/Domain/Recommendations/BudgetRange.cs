namespace Domain.Recommendations;

public sealed record BudgetRange(decimal Min, decimal Max)
{
    public BudgetRange() : this(0, 0) { }

    public bool Fits(decimal price) => price >= Min && price <= Max;

    public void Validate()
    {
        if (Min < 0 || Max < Min)
        {
            throw new ArgumentException("Некорректный диапазон бюджета");
        }
    }
}
