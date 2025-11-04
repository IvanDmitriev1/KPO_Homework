using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Infrastructure.Abstractions;
using Spectre.Console;

namespace KPO_HW2.App.Commands.Account;

internal class CreateAccountCommand(IAccountService accountService) : ICommand
{
    public string Name => "Создать счёт";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var name = AnsiConsole.Ask<string>("Введите [green]название счёта[/]:");
        var initialBalance = await AnsiConsole.AskAsync<decimal>("Введите [green]начальный баланс[/]:");

        var id = await accountService.CreateAccount(name, initialBalance);

        AnsiConsole.MarkupLine(
            $"[green]Счёт создан[/] c id [yellow]{id}[/]");
    }
}