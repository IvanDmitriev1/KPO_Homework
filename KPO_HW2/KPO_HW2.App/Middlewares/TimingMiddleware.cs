using System.Diagnostics;
using KPO_HW2.App.CommandsAbstractions;
using Spectre.Console;

namespace KPO_HW2.App.Middlewares;

internal class TimingMiddleware : ICommandMiddleware
{
    public async Task InvokeAsync(
        ICommand command,
        CancellationToken ct,
        Func<CancellationToken, Task> next)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await next(ct);
        }
        finally
        {
            sw.Stop();
            AnsiConsole.MarkupLine(
                $"[grey]Команда '{command.Name}' заняла {sw.ElapsedMilliseconds} мс[/]");
        }
    }
}
