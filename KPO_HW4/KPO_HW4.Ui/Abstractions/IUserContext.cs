namespace KPO_HW4.Ui.Abstractions;

public interface IUserContext
{
    UserId UserId { get; }

    Task Initialize(CancellationToken ct);
}