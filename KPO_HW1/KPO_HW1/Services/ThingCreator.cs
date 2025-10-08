namespace KPO_HW1.Services;

internal class ThingCreator<T> : IThingCreator
    where T : Thing
{
    public ThingCreator(IInventoryNumberProvider numberProvider)
    {
        _numberProvider = numberProvider;
        Type = typeof(T).Name;
    }

    private readonly IInventoryNumberProvider _numberProvider;

    public string Type { get; }

    public Thing Create()
    {
        var thing = (Thing?)Activator.CreateInstance(typeof(T), _numberProvider.Next());
        return thing ?? throw new InvalidOperationException($"Cannot create instance of type {typeof(T).FullName}");
    }
}