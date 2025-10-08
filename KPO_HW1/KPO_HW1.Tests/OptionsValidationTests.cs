namespace KPO_HW1.Tests;

public class OptionsValidationTests
{
    [Fact]
    public void AnimalCreateOptions_validate_throws_on_invalid_values()
    {
        var zeroNumber = new AnimalCreateOptions { Name = "X", FoodKgPerDay = 1, HealthPercent = 1, Number = 0 };
        Assert.Throws<ArgumentOutOfRangeException>(() => zeroNumber.Validate());

        var emptyName = new AnimalCreateOptions { Name = "", FoodKgPerDay = 1, HealthPercent = 1, Number = 1 };
        Assert.Throws<ArgumentException>(() => emptyName.Validate());

        var zeroFood = new AnimalCreateOptions { Name = "X", FoodKgPerDay = 0, HealthPercent = 1, Number = 1 };
        Assert.Throws<ArgumentOutOfRangeException>(() => zeroFood.Validate());

        var zeroHealth = new AnimalCreateOptions { Name = "X", FoodKgPerDay = 1, HealthPercent = 0, Number = 1 };
        Assert.Throws<ArgumentOutOfRangeException>(() => zeroHealth.Validate());
    }

    [Fact]
    public void HerbivoreAnimalCreateOptions_validate_requires_positive_friendlies()
    {

        var bad = new HerbivoreAnimalCreateOptions
            { Name = "Herb", FoodKgPerDay = 1, HealthPercent = 1, Number = 1, Friendlies = 0 };
        Assert.Throws<ArgumentOutOfRangeException>(() => bad.Validate());

        var ok = new HerbivoreAnimalCreateOptions
            { Name = "Ok", FoodKgPerDay = 1, HealthPercent = 1, Number = 1, Friendlies = 1 };
        ok.Validate();
    }
}