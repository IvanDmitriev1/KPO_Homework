using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using KPO_HW2.Models;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Account;

internal class EditAccountCommand(IAccountService accountService) : ICommand
{
    public string Name => "Переименовать счёт";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var accounts = await accountService.GetAllAccounts();
        if (accounts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Нет счетов для редактирования[/]");
            return;
        }

        var selected = await AnsiConsole.PromptAsync(
            new SelectionPrompt<BankAccount>()
                .Title("Выберите [green]счёт[/] для переименования:")
                .UseConverter(a => $"{a.Name} (баланс: {a.Balance})")
                .AddChoices(accounts));

        var newName = await AnsiConsole.AskAsync<string>(
            $"Введите новое название для счёта [yellow]{selected.Name}[/]:");


        await accountService.UpdateAccount(selected.Id, newName);

        AnsiConsole.MarkupLine(
            $"Счёт переименован в [green]{newName}[/]");
    }
}