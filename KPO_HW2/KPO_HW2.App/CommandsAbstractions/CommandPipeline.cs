using Microsoft.Extensions.DependencyInjection;

namespace KPO_HW2.App.CommandsAbstractions;

internal class CommandPipeline
{
    public CommandPipeline(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private readonly IServiceScopeFactory _scopeFactory;

    public async Task ExecuteAsync(CommandDescriptor descriptor, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var middlewares = sp.GetServices<ICommandMiddleware>().ToList();
        var command = (ICommand)sp.GetRequiredService(descriptor.CommandType);

        await InvokeRecursive(0, middlewares, command, ct);
    }

    private static Task InvokeRecursive(
        int index,
        IReadOnlyList<ICommandMiddleware> middlewares,
        ICommand command,
        CancellationToken ct)
    {
        if (index == middlewares.Count)
            return command.ExecuteAsync(ct);

        var current = middlewares[index];

        return current.InvokeAsync(
            command,
            ct,
            nextCt => InvokeRecursive(index + 1, middlewares, command, nextCt));
    }
}