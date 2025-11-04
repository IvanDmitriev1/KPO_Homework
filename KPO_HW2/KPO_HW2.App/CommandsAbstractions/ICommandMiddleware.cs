namespace KPO_HW2.App.CommandsAbstractions;

internal interface ICommandMiddleware
{
    Task InvokeAsync(
        ICommand command,
        CancellationToken ct,
        Func<CancellationToken, Task> next);
}