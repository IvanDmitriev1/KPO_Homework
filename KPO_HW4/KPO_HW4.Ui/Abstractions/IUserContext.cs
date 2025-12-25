namespace KPO_HW4.Ui.Abstractions;

public interface IUserContext
{
    UserId UserId { get; }

    ValueTask Initialize(CancellationToken ct);
}