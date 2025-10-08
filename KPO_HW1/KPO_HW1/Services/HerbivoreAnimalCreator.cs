using Spectre.Console;

namespace KPO_HW1.Services;

internal class HerbivoreAnimalCreator<T> : IAnimalCreator
    where T : HerbivoreAnimal
{
    public HerbivoreAnimalCreator(IInventoryNumberProvider numbersProvider)
    {
        _numbersProvider = numbersProvider;
        Species = typeof(T).Name;
    }

    private readonly IInventoryNumberProvider _numbersProvider;

    public string Species { get; }

    public Animal Create()
    {
        var name = AnsiConsole.Prompt(new TextPrompt<string>("Имя:")
            .Validate(n => string.IsNullOrWhiteSpace(n)
                ? ValidationResult.Error("Имя обязательно")
                : ValidationResult.Success()));

        var food = AnsiConsole.Prompt(new TextPrompt<int>("Кг еды в сутки:")
            .DefaultValue(1)
            .Validate(v => v < 0
                ? ValidationResult.Error("Значение не может быть отрицательным")
                : ValidationResult.Success()));

        var hp = AnsiConsole.Prompt(new TextPrompt<int>("Здоровье, % (0..100):")
            .DefaultValue(80)
            .Validate(v => v is < 0 or > 100
                ? ValidationResult.Error("Должно быть в диапазоне 0..100")
                : ValidationResult.Success()));

        var fr = AnsiConsole.Prompt(new TextPrompt<int>("Доброта 0..10:")
            .DefaultValue(7)
            .Validate(v => v is < 0 or > 10
                ? ValidationResult.Error("Должно быть в диапазоне 0..10")
                : ValidationResult.Success()));

        HerbivoreAnimalCreateOptions options = new HerbivoreAnimalCreateOptions()
        {
            Name = name,
            FoodKgPerDay = food,
            HealthPercent = hp,
            Friendlies = fr,
            Number = _numbersProvider.Next()
        };

        var animal = (Animal?)Activator.CreateInstance(typeof(T), options);
        return animal ?? throw new InvalidOperationException($"Не удалось создать экземпляр {typeof(T).Name}.");
    }
}