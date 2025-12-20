namespace KPO_HW4.Ui.Abstractions;

public interface IOrdersRealtimeClient : IAsyncDisposable
{
    Task ConnectAsync(UserId userId, CancellationToken ct = default);
    Task DisconnectAsync(CancellationToken ct = default);

    IDisposable SubscribeToOrderStatusChange(Action<OrderStatusChangedPush> handler);
}