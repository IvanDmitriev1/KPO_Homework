using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Operations;

internal class CreateOperationCommand(
    IAccountService accountService,
    ICategoryService categoryService,
    IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Создать операцию (доход/расход)";

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

        var allCategories = await categoryService.GetAll();

        var category = await AnsiConsole.PromptAsync(
            new SelectionPrompt<Models.Category>()
                .Title("Выберите [green]категорию[/]:")
                .UseConverter(c => $"{c.Name} ({c.CategoryType})")
                .AddChoices(allCategories));

        var amount = await AnsiConsole.AskAsync<decimal>("Введите [green]сумму[/]:");
        var date = await AnsiConsole.AskAsync<DateTime>("Введите [green]дату[/] (например, 2025-11-04):", DateTime.Now);
        var description = await AnsiConsole.AskAsync<string>("Введите [green]описание[/]:");

        var moneyAmount = Money.Create(amount, account.Balance.CurrencyCode);

        await accountOperationService.CreateOperation(account.Id,
            category.Id,
            moneyAmount,
            date.ToUniversalTime(),
            description);

        AnsiConsole.MarkupLine(
            $"[green]Операция создана[/]: [yellow]{category.CategoryType}[/] {moneyAmount} на счёте [yellow]{account.Name}[/].");
    }
}