using FluentValidation;
using FluentValidation.Results;
using KPO_HW2.App.CommandsAbstractions;
using Spectre.Console;

namespace KPO_HW2.App.Middlewares;

internal class ValidationExceptionMiddleware : ICommandMiddleware
{
    public async Task InvokeAsync(
        ICommand command,
        CancellationToken ct,
        Func<CancellationToken, Task> next)
    {
        try
        {
            await next(ct);
        }
        catch (ValidationException ex)
        {
            AnsiConsole.MarkupLine(
                "[red]Валидация не пройдена для команды[/] [yellow]{0}[/]:",
                command.Name);


            var failures = ex.Errors ?? Enumerable.Empty<ValidationFailure>();

            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Red)
                .AddColumn("[yellow]Свойство[/]")
                .AddColumn("[yellow]Ошибка[/]")
                .AddColumn("[yellow]Введённое значение[/]");

            foreach (var failure in failures)
            {
                table.AddRow(
                    failure.PropertyName ?? "",
                    failure.ErrorMessage ?? "",
                    failure.AttemptedValue?.ToString() ?? "<пусто>");
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("[grey]Нажмите любую клавишу, чтобы продолжить...[/]");
            Console.ReadKey(intercept: true);
        }
    }
}