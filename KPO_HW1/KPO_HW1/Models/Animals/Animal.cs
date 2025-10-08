namespace KPO_HW1.Models.Animals;

internal abstract class Animal : IAlive, IInventory
{
    protected Animal(AnimalCreateOptions options)
    {
        options.Validate();

        Name = options.Name;
        FoodKgPerDay = options.FoodKgPerDay;
        HealthPercent = options.HealthPercent;
        Number = options.Number;
    }

    public abstract string Species { get; }

    public string Name { get; }
    public int FoodKgPerDay { get; }
    public int HealthPercent { get; }
    public int Number { get; }
}