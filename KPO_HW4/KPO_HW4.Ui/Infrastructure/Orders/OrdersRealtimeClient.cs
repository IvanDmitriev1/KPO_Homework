using Microsoft.AspNetCore.SignalR.Client;

namespace KPO_HW4.Ui.Infrastructure.Orders;

public sealed class OrdersRealtimeClient : IOrdersRealtimeClient
{
    public OrdersRealtimeClient(Uri baseAddress)
    {
        _baseAddress = baseAddress;
    }

    private readonly Uri _baseAddress;
    private HubConnection? _hub;

    public async ValueTask DisposeAsync() => await DisconnectAsync();

    public async Task ConnectAsync(UserId userId, CancellationToken ct = default)
    {
        if (_hub?.State is HubConnectionState.Connected or HubConnectionState.Connecting)
            return;

        var hubUrl = new Uri(_baseAddress, $"hub/orderNotifications/{userId.Value:D}");

        _hub = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        await _hub.StartAsync(ct);
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

    public IDisposable SubscribeToOrderStatusChange(Action<OrderStatusChangedPush> handler)
    {
        EnsureConnected();

        return _hub!.On(OrderStatusChangedPush.Name, handler);
    }

    private void EnsureConnected()
    {
        if (_hub is null || _hub.State != HubConnectionState.Connected)
            throw new InvalidOperationException("Realtime client is not connected. Call ConnectAsync first.");
    }
}