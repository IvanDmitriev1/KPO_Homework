namespace KPO_HW1.Tests;

public class AnimalFactoryTests
{
    [Fact]
    public void CreateAnimal_returns_instance()
    {
        var expected = new Tiger(new AnimalCreateOptions { Name = "X", FoodKgPerDay = 1, HealthPercent = 80, Number = 1 });
        var creator = Substitute.For<IAnimalCreator>();
        creator.Species.Returns("sp");
        creator.Create().Returns(expected);

        var factory = new AnimalFactory([creator]);

        var actual = factory.CreateAnimal("sp");

        Assert.Same(expected, actual);
    }
}