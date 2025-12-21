using Microsoft.AspNetCore.SignalR.Client;

namespace KPO_HW4.Ui.Infrastructure.Orders;

public sealed class OrdersRealtimeClient : IOrdersRealtimeClient
{
    public OrdersRealtimeClient(Uri baseAddress)
    {
        _baseAddress = baseAddress;
    }

    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Uri _baseAddress;
    private HubConnection? _hub;
    private Task? _connectTask;

    public async ValueTask DisposeAsync() => await DisconnectAsync();

    public async Task ConnectAsync(UserId userId, CancellationToken ct = default)
    {
        Task task;

        await _lock.WaitAsync(ct);
        try
        {
            if (_hub?.State == HubConnectionState.Connected)
                return;

            _hub ??= BuildHub(userId);

            task = _connectTask is { IsCompleted: false }
                ? _connectTask
                : _connectTask = _hub.StartAsync(ct);
        }
        finally
        {
            _lock.Release();
        }

        await task;
    }

    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        if (_hub is null) 
            return;

        try
        {
            await _hub.StopAsync(ct);
        }
        finally
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }

    public IDisposable SubscribeToOrderStatusChange(Func<OrderStatusChangedPush, Task> handler)
    {
        EnsureConnected();
        return _hub!.On(OrderStatusChangedPush.Name, handler);
    }

    public IDisposable SubscribeToOrderStatusChange(Action<OrderStatusChangedPush> handler)
    {
        EnsureConnected();
        return _hub!.On(OrderStatusChangedPush.Name, handler);
    }

    private HubConnection BuildHub(UserId userId)
    {
        var hubUrl = new Uri(_baseAddress, $"/api/orders/hub/orderNotifications/{userId.Value:D}");
        return new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();
    }

    private void EnsureConnected()
    {
        if (_hub is null || _hub.State != HubConnectionState.Connected)
            throw new InvalidOperationException("Realtime client is not connected. Call ConnectAsync first.");
    }
}