namespace KPO_HW4.Ui.Abstractions;

public interface IUserContext
{
    ValueTask<UserId> GetUserIdAsync(CancellationToken ct);
}