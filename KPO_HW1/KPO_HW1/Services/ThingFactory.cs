namespace KPO_HW1.Services;

internal class ThingFactory : IThingFactory
{
    public ThingFactory(IEnumerable<IThingCreator> thingCreators)
    {
        var creators = thingCreators.ToList();
        KnownThings = creators.Select(static c => c.Type).ToList();
        _registry = creators.ToDictionary(static c => c.Type, StringComparer.OrdinalIgnoreCase);
    }

    private readonly IReadOnlyDictionary<string, IThingCreator> _registry;

    public IReadOnlyCollection<string> KnownThings { get; }

    public Thing CreateThing(string type)
    {
        return _registry[type].Create();
    }
}