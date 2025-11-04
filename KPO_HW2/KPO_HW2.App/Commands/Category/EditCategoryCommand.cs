using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Category;

internal class EditCategoryCommand(ICategoryService categoryService) : ICommand
{
    public string Name => "Переименовать категорию";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var cats = await categoryService.GetAll();
        if (cats.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет категорий для редактирования[/]");
            return;
        }

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<Models.Category>()
                .Title("Выберите [green]категорию[/] для переименования:")
                .UseConverter(c => $"{c.Name} ({c.CategoryType})")
                .AddChoices(cats));

        var newName = await AnsiConsole.AskAsync<string>(
            $"Новое название для категории [yellow]{selected.Name}[/]:");

        await categoryService.UpdateCategory(selected.Id, newName);
        AnsiConsole.MarkupLine("[green]Категория переименована[/]");
    }
}