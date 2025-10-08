namespace KPO_HW1.Abstractions;

internal interface IThingFactory
{
    IReadOnlyCollection<string> KnownThings { get; }

    Thing CreateThing(string type);
}