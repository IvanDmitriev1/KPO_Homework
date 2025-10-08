namespace KPO_HW1.Abstractions;

internal interface IAnimalFactory
{
    IReadOnlyCollection<string> KnownSpecies { get; }

    Animal CreateAnimal(string species);
}