namespace KPO_HW1.Tests;

public class ZooTests
{
    private static HerbivoreAnimalCreateOptions HerboCreateOptions(string name, int food, int hp, int fr) =>
        new() { Name = name, FoodKgPerDay = food, HealthPercent = hp, Friendlies = fr, Number = 1 };

    private static AnimalCreateOptions AnimalCreateOptions(string name, int food, int hp) =>
        new() { Name = name, FoodKgPerDay = food, HealthPercent = hp, Number = 1 };

    [Fact]
    public void TryAdmit_accepts_healthy_animal_and_updates_food_sum()
    {
        var clinic = Substitute.For<IVeterinaryClinic>();
        clinic.IsHealthy(Arg.Any<Animal>()).Returns(true);

        var zoo = new Zoo(clinic);
        var rabbit = new Rabbit(HerboCreateOptions("Roger", 2, 90, 8));

        var accepted = zoo.TryAdmit(rabbit);

        Assert.True(accepted);
        Assert.Contains(rabbit, zoo.Animals);
        Assert.Equal(2, zoo.TotalFoodKgPerDay);
    }

    [Fact]
    public void TryAdmit_rejects_unhealthy_animal_and_does_not_change_state()
    {
        var clinic = Substitute.For<IVeterinaryClinic>();
        clinic.IsHealthy(Arg.Any<Animal>()).Returns(false);

        var zoo = new Zoo(clinic);
        var tiger = new Tiger(AnimalCreateOptions("Shere Khan", 6, 50));

        var accepted = zoo.TryAdmit(tiger);

        Assert.False(accepted);
        Assert.Empty(zoo.Animals);
        Assert.Equal(0, zoo.TotalFoodKgPerDay);
    }

    [Fact]
    public void TotalFoodKgPerDay_accumulates_over_multiple_admissions()
    {
        var clinic = Substitute.For<IVeterinaryClinic>();
        clinic.IsHealthy(Arg.Any<Animal>()).Returns(true);

        var zoo = new Zoo(clinic);
        var r1 = new Rabbit(HerboCreateOptions("R1", 2, 90, 7));
        var t1 = new Tiger(AnimalCreateOptions("T1", 6, 80));

        Assert.True(zoo.TryAdmit(r1));
        Assert.True(zoo.TryAdmit(t1));

        Assert.Equal(8, zoo.TotalFoodKgPerDay); // 2 + 6
    }

    [Fact]
    public void GetPettingZooCandidates_returns_only_herbivores_with_kindness_threshold()
    {
        var clinic = Substitute.For<IVeterinaryClinic>();
        clinic.IsHealthy(Arg.Any<Animal>()).Returns(true);

        var zoo = new Zoo(clinic);
        var niceBunny = new Rabbit(HerboCreateOptions("Bunny", 1, 90, 6));
        var shyBunny = new Rabbit(HerboCreateOptions("Shy", 1, 90, 5));
        var wolf = new Wolf(AnimalCreateOptions("Wolfy", 4, 90));

        zoo.TryAdmit(niceBunny);
        zoo.TryAdmit(shyBunny);
        zoo.TryAdmit(wolf);

        var petting = zoo.GetPettingZooCandidates().ToList();

        Assert.Contains(niceBunny, petting);
        Assert.DoesNotContain(shyBunny, petting);
    }

    [Fact]
    public void AddAsset_adds_inventory()
    {
        var clinic = Substitute.For<IVeterinaryClinic>();
        var zoo = new Zoo(clinic);

        var table = new Table(100);
        var pc = new Computer(101);

        zoo.AddAsset(table);
        zoo.AddAsset(pc);

        Assert.Contains(table, zoo.Assets);
        Assert.Contains(pc, zoo.Assets);
    }
}