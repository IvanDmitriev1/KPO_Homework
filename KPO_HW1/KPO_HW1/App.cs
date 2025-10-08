using Spectre.Console;
using Table = Spectre.Console.Table;

namespace KPO_HW1;

internal class App
{
    public App(
        IZoo zoo,
        IAnimalFactory animalFactory,
        IThingFactory thingFactory)
    {
        _zoo = zoo;
        _animalFactory = animalFactory;
        _thingFactory = thingFactory;
    }

    private readonly IZoo _zoo;
    private readonly IAnimalFactory _animalFactory;
    private readonly IThingFactory _thingFactory;

    public void Run()
    {
        while (true)
        {
            try
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Что делаем?[/]")
                        .AddChoices(
                            "Добавить животное",
                            "Показать животных",
                            "Кандидаты в контакт‑зоопарк",
                            "Сумма еды (кг/сутки)",
                            "Добавить вещь",
                            "Показать вещи",
                            "Инвентарная опись",
                            "Выход"));

                AnsiConsole.Clear();

                switch (choice)
                {
                    case "Добавить животное":
                        AddAnimal();
                        break;
                    case "Показать животных":
                        ShowAnimals();
                        break;
                    case "Кандидаты в контакт‑зоо":
                        ShowPettingCandidates();
                        break;
                    case "Сумма еды (кг/сутки)":
                        ShowTotalFood();
                        break;
                    case "Добавить вещь":
                        AddAsset();
                        break;
                    case "Показать вещи":
                        ShowAssets();
                        break;
                    case "Инвентарная опись":
                        ShowInventory();
                        break;
                    case "Выход":
                        return;
                }
            }
            catch (Exception e)
            {
                AnsiConsole.WriteLine("[green]Произошла ошибка[/]");
            }
        }
    }

    private void AddAnimal()
    {
        var species = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Вид животного[/]")
                .AddChoices(_animalFactory.KnownSpecies));

        var animal = _animalFactory.CreateAnimal(species);

        var accepted = _zoo.TryAdmit(animal);
        AnsiConsole.MarkupLine(accepted
            ? $"[green]Принято в зоопарк:[/] [bold]{animal.Name}[/] (#{animal.Number}, {species})"
            : "[yellow]Отклонено ветеринарной клиникой[/]");
    }

    private void ShowAnimals()
    {
        if (_zoo.Animals.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]Пока нет животных[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("#")
            .AddColumn("Вид")
            .AddColumn("Имя")
            .AddColumn("Еда кг/сут")
            .AddColumn("Здоровье %")
            .AddColumn("Доброта (если есть)");

        foreach (var a in _zoo.Animals)
        {
            var friendlies = a is HerbivoreAnimal h ? h.Friendlies.ToString() : "—";
            table.AddRow(a.Number.ToString(), a.Species, a.Name, a.FoodKgPerDay.ToString(), a.HealthPercent.ToString(), friendlies);
        }

        AnsiConsole.Write(table);
    }

    private void ShowPettingCandidates()
    {
        var list = _zoo.GetPettingZooCandidates().ToList();
        if (list.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]Кандидатов нет[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Ascii)
            .AddColumn("#")
            .AddColumn("Имя")
            .AddColumn("Доброта")
            .AddColumn("Еда кг/сут");

        foreach (var h in list)
        {
            table.AddRow(h.Number.ToString(), h.Name, h.Friendlies.ToString(), h.FoodKgPerDay.ToString());
        }


        AnsiConsole.Write(table);
    }

    private void ShowTotalFood()
    {
        var total = _zoo.TotalFoodKgPerDay;
        AnsiConsole.MarkupLine($"Еды на сутки: [bold]{total} кг[/]");
    }

    private void AddAsset()
    {
        var species = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Тип вещи:[/]")
                .AddChoices(_thingFactory.KnownThings));

        var asset = _thingFactory.CreateThing(species);
        _zoo.AddAsset(asset);

        AnsiConsole.MarkupLine("[green]Вещь добавлена[/]");
    }

    private void ShowAssets()
    {
        if (_zoo.Assets.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]Пока нет вещей[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("#")
            .AddColumn("Тип");


        foreach (var a in _zoo.Assets)
        {
            table.AddRow(a.Number.ToString(), a.Type);
        }

        AnsiConsole.Write(table);
    }

    private void ShowInventory()
    {
        var table = new Table().RoundedBorder();
        table.AddColumn("№");
        table.AddColumn("Название");
        table.AddColumn("Тип");

        var sortedItems = _zoo.Animals
            .Cast<IInventory>()
            .Concat(_zoo.Assets)
            .OrderBy(static i => i.Number);

        foreach (var item in sortedItems)
        {
            switch (item)
            {
                case Animal animalItem:
                    table.AddRow(animalItem.Number.ToString(), animalItem.Name, "Animal");
                    break;
                case Thing thingItem:
                    table.AddRow(thingItem.Number.ToString(), thingItem.Type, "Thing");
                    break;
            }
        }

        AnsiConsole.Write(table);
    }
}