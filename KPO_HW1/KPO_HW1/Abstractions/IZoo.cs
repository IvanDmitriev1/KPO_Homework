namespace KPO_HW1.Abstractions;

internal interface IZoo
{
    IReadOnlyCollection<Animal> Animals { get; }
    IReadOnlyCollection<Thing> Assets { get; }
    int TotalFoodKgPerDay { get; }

    bool TryAdmit(Animal animal);
    void AddAsset(Thing asset);

    IEnumerable<HerbivoreAnimal> GetPettingZooCandidates();
}