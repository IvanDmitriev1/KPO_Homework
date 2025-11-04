using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Account;

internal class DeleteAccountCommand(IAccountService accountService, IAccountOperationService accountOperationService) : ICommand
{
    public string Name => "Удалить счёт";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var accounts = await accountService.GetAllAccounts();
        if (accounts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет счетов для удаления[/]");
            return;
        }

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<BankAccount>()
                .Title("Выберите [green]счёт[/] для удаления:")
                .UseConverter(a => $"{a.Name} (баланс: {a.Balance})")
                .AddChoices(accounts));

        var ops = await accountOperationService.GetByAccount(selected.Id);
        if (ops.Any())
        {
            AnsiConsole.MarkupLine(
                "[red]Нельзя удалить счёт с существующими операциями[/]");
            return;
        }

        if (!await AnsiConsole.ConfirmAsync(
                $"Точно удалить счёт [yellow]{selected.Name}[/]?"))
            return;

        await accountService.DeleteAccount(selected.Id);
        AnsiConsole.MarkupLine("[green]Счёт удалён[/]");
    }
}