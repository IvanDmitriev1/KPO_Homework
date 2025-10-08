namespace KPO_HW1.Services;

internal class AnimalFactory : IAnimalFactory
{
    public AnimalFactory(IEnumerable<IAnimalCreator> animalCreators)
    {
        var creators = animalCreators.ToList();
        _registry = creators.ToDictionary(static c => c.Species, StringComparer.OrdinalIgnoreCase);
        KnownSpecies = creators.Select(static c => c.Species).ToList();
    }

    private readonly IReadOnlyDictionary<string, IAnimalCreator> _registry;
    public IReadOnlyCollection<string> KnownSpecies { get; }

    public Animal CreateAnimal(string species)
    {
        return _registry[species].Create();
    }
}