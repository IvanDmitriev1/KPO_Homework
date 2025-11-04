namespace KPO_HW2.App.CommandsAbstractions;

internal interface ICommand
{
    string Name { get; }

    Task ExecuteAsync(CancellationToken ct);
}