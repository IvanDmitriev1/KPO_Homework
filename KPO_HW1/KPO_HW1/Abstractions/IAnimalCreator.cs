namespace KPO_HW1.Abstractions;

internal interface IAnimalCreator
{
    string Species { get; }

    Animal Create();
}