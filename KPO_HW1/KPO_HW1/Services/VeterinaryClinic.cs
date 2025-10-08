namespace KPO_HW1.Services;

internal class VeterinaryClinic : IVeterinaryClinic
{
    public VeterinaryClinic(int meanHealth)
    {
        _meanHealth = meanHealth;
    }

    private readonly int _meanHealth;

    public bool IsHealthy(Animal animal)
    {
        return animal.HealthPercent >= _meanHealth;
    }
}