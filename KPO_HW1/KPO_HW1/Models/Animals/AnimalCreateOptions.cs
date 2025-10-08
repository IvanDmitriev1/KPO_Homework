namespace KPO_HW1.Models.Animals;

internal record AnimalCreateOptions
{
    public required string Name { get; init; }
    public required int FoodKgPerDay { get; init; }
    public required int HealthPercent { get; init; }
    public required int Number { get; init; }

    public virtual void Validate()
    {
        ArgumentException.ThrowIfNullOrEmpty(Name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(FoodKgPerDay);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(HealthPercent);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Number);
    }
}