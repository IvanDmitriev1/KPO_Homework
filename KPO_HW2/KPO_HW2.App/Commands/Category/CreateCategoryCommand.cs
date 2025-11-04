using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Category;

internal class CreateCategoryCommand(ICategoryService categoryService) : ICommand
{
    public string Name => "Создать категорию";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var type = await AnsiConsole.PromptAsync(
            new SelectionPrompt<CategoryType>()
                .Title("Тип категории:")
                .AddChoices(CategoryType.Income, CategoryType.Expense));

        var name = AnsiConsole.Ask<string>("Название [green]категории[/]:");

        await categoryService.CreateCategory(name, type);

        AnsiConsole.MarkupLine(
            $"[green]Категория создана[/]: [yellow]{name}[/] ({type})");
    }
}