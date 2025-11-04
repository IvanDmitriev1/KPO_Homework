using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Category;

internal class DeleteCategoryCommand(ICategoryService categoryService, IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Удалить категорию";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var cats = await categoryService.GetAll();
        if (cats.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет категорий для удаления[/]");
            return;
        }

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<Models.Category>()
                .Title("Выберите [green]категорию[/] для удаления:")
                .UseConverter(c => $"{c.Name} ({c.CategoryType})")
                .AddChoices(cats));

        bool operationExists = await accountOperationService.HasOperationsWithCategory(selected.Id);
        if (operationExists)
        {
            AnsiConsole.MarkupLine(
                "[red]Нельзя удалить категорию, к которой привязаны операции[/]");
            return;
        }

        if (!await AnsiConsole.ConfirmAsync(
                $"Точно удалить категорию [yellow]{selected.Name}[/]?"))
            return;

        await categoryService.DeleteCategory(selected.Id);
        AnsiConsole.MarkupLine("[green]Категория удалена[/]");
    }
}