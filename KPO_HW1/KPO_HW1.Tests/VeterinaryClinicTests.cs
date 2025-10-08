namespace KPO_HW1.Tests;

public class VeterinaryClinicTests
{
    private static AnimalCreateOptions AnimalCreateOptions(int hp) =>
        new() { Name = "X", FoodKgPerDay = 1, HealthPercent = hp, Number = 1 };

    [Fact]
    public void IsHealthy_true()
    {
        var clinic = new VeterinaryClinic(meanHealth: 70);
        Assert.True(clinic.IsHealthy(new Tiger(AnimalCreateOptions(70))));
        Assert.True(clinic.IsHealthy(new Tiger(AnimalCreateOptions(90))));
    }

    [Fact]
    public void IsHealthy_false()
    {
        var clinic = new VeterinaryClinic(meanHealth: 70);
        Assert.False(clinic.IsHealthy(new Tiger(AnimalCreateOptions(69))));
        Assert.False(clinic.IsHealthy(new Tiger(AnimalCreateOptions(1))));
    }
}