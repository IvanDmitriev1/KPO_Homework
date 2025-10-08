namespace KPO_HW1.Abstractions;

internal interface IThingCreator
{
    string Type { get; }

    Thing Create();
}