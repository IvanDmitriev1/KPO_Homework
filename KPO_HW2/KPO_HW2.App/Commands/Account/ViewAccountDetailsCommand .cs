using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Account;

internal class ViewAccountDetailsCommand(
    IAccountService accountService,
    ICategoryService categoryService,
    IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Посмотреть баланс и операции по счёту";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var allAccounts = await accountService.GetAllAccounts();
        if (!allAccounts.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Счётов пока нет.[/]");
            return;
        }

        var account = AnsiConsole.Prompt(
            new SelectionPrompt<BankAccount>()
                .Title("Выберите [green]счёт[/]:")
                .UseConverter(a => $"{a.Name} (баланс: {a.Balance})")
                .AddChoices(allAccounts));

        var header = new Panel(
            new Markup(
                $"Счёт: [yellow]{account.Name}[/]\n" +
                $"Id: [grey]{account.Id}[/]\n" +
                $"Баланс: [green]{account.Balance}[/]"))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("Информация о счёте")
        };

        AnsiConsole.Write(header);



        var accountOps = await accountOperationService.GetByAccount(account.Id);
        if (accountOps.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет ни одной операции[/].");
            return;
        }

        var allCategories = await categoryService.GetAll();
        var categoriesById = allCategories.ToDictionary(c => c.Id, c => c);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Операции по счёту");

        table.AddColumn("Дата");
        table.AddColumn("Тип");
        table.AddColumn("Сумма");
        table.AddColumn("Категория");
        table.AddColumn("Описание");

        foreach (var op in accountOps)
        {
            if (!categoriesById.TryGetValue(op.CategoryId, out var cat))
                continue;

            var catName = cat.Name;
            var catType = cat.CategoryType.ToString();

            var isIncome = cat.CategoryType== CategoryType.Income;
            var color = isIncome ? "green" : "red";
            var sign = isIncome ? "+" : "-";

            var amountText = $"[{color}]{sign}{op.Amount}[/]";

            table.AddRow(
                op.DateOfOperation.ToString("yyyy-MM-dd"),
                catType,
                amountText,
                catName,
                op.Description);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
    }
}