namespace KPO_HW1.Models.Animals;

internal abstract class HerbivoreAnimal : Animal, IHerbivore
{
    protected HerbivoreAnimal(HerbivoreAnimalCreateOptions options) : base(options)
    {
        Friendlies = options.Friendlies;
    }

    public int Friendlies { get; }
}