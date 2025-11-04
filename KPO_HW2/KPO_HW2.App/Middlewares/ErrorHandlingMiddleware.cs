using KPO_HW2.App.CommandsAbstractions;
using KPO_HW2.Exceptions;
using Spectre.Console;

namespace KPO_HW2.App.Middlewares;

internal class ErrorHandlingMiddleware : ICommandMiddleware
{
    public async Task InvokeAsync(ICommand command, CancellationToken ct, Func<CancellationToken, Task> next)
    {
        try
        {
            await next(ct);
        }
        catch (DuplicateException ex)
        {
            var entity = ex.EntityName;
            var field = ex.FieldName;
            var value = ex.Value;

            AnsiConsole.MarkupLine(
                $"[red]Невозможно выполнить команду[/] [yellow]{command.Name}[/].");
            AnsiConsole.MarkupLine(
                $"{entity} с {field} [yellow]{value}[/] уже существует.");

            AnsiConsole.MarkupLine(
                "[grey]Подсказка: выберите другое имя.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(
                $"[red]Произошла непредвиденная ошибка при выполнении команды[/] [yellow]{command.Name}[/].");

            //AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
    }
}