namespace KPO_HW1.Services;

internal class Zoo : IZoo
{
    public Zoo(IVeterinaryClinic veterinaryClinic)
    {
        _veterinaryClinic = veterinaryClinic;
    }

    private readonly IVeterinaryClinic _veterinaryClinic;
    private readonly List<Animal> _animals = [];
    private readonly List<Thing> _assets = [];

    public IReadOnlyCollection<Animal> Animals => _animals;
    public IReadOnlyCollection<Thing> Assets => _assets;
    public int TotalFoodKgPerDay { get; private set; }

    public bool TryAdmit(Animal animal)
    {
        if (!_veterinaryClinic.IsHealthy(animal))
        {
            return false;
        }

        _animals.Add(animal);
        TotalFoodKgPerDay = _animals.Sum(static a => a.FoodKgPerDay);
        return true;
    }

    public void AddAsset(Thing asset)
    {
        _assets.Add(asset);
    }

    public IEnumerable<HerbivoreAnimal> GetPettingZooCandidates()
    {
        return _animals.OfType<HerbivoreAnimal>().Where(h => h.Friendlies >= 6);
    }
}