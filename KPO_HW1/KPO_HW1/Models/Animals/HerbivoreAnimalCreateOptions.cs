namespace KPO_HW1.Models.Animals;

internal record HerbivoreAnimalCreateOptions : AnimalCreateOptions
{
    public required int Friendlies { get; init; }

    public override void Validate()
    {
        base.Validate();

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Friendlies);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Friendlies, 10);
    }
}