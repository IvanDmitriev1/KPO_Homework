using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Operations;

internal class DeleteOperationCommand(
    IAccountService accountService,
    ICategoryService categoryService,
    IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Удалить операцию";

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

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<AccountOperation>()
                .Title("Выберите [red]операцию для удаления[/]:")
                .UseConverter(operation => $"{operation.DateOfOperation:yy-MM-dd} | {operation.Description} | {operation.Amount}")
                .AddChoices(operations));

        var confirm = await AnsiConsole.ConfirmAsync(
            $"Точно удалить операцию [yellow]{selected.DateOfOperation:yy-MM-dd}[/] ({selected.Description} {selected.Amount})?");

        if (!confirm)
            return;

        await accountOperationService.DeleteOperation(selected.Id);
        AnsiConsole.MarkupLine("[green]Операция удалена[/].");
    }
}