using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Operations;

internal class EditOperationCommand(
    IAccountService accountService,
    ICategoryService categoryService,
    IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Редактировать операцию";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var accounts = await accountService.GetAllAccounts();
        if (!accounts.Any())
        {
            AnsiConsole.MarkupLine("[red]Нет ни одного счёта[/] — сначала создайте счёт.");
            return;
        }

        var account = await AnsiConsole.PromptAsync(
            new SelectionPrompt<BankAccount>()
                .Title("Выберите [green]счёт[/] для операции:")
                .UseConverter(a => $"{a.Name} (баланс: {a.Balance})")
                .AddChoices(accounts));

        var operations = await accountOperationService.GetByAccount(account.Id);
        if (operations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет ни одной операции[/].");
            return;
        }

        var categories = (await categoryService.GetAll()).ToDictionary(c => c.Id);

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<AccountOperation>()
                .Title("Выберите [green]операцию для редактирования[/]:")
                .UseConverter(OpToString)
                .AddChoices(operations));

        var newAmount = await AnsiConsole.AskAsync<decimal>(
            $"Новая сумма (текущая: [yellow]{selected.Amount}[/]):");

        var newDescription = await AnsiConsole.AskAsync<string>(
            $"Новое описание (текущее: [yellow]{selected.Description}[/]):");

        var newMoney = Money.Create(newAmount, account.Balance.CurrencyCode);
        await accountOperationService.UpdateOperation(selected.Id, newMoney, newDescription);

        AnsiConsole.MarkupLine("[green]Операция обновлена[/].");
        return;

        string OpToString(AccountOperation o)
        {
            var cat = categories[o.CategoryId];
            var catName = cat.Name;
            var catType = cat.CategoryType.ToString();

            var isIncome = cat.CategoryType == CategoryType.Income;
            var color = isIncome ? "green" : "red";
            var sign = isIncome ? "+" : "-";

            return $"{o.DateOfOperation:yyyy-MM-dd} | {catType} | [{color}]{sign}{o.Amount}[/] | {catName} | {o.Description}";
        }
    }
}