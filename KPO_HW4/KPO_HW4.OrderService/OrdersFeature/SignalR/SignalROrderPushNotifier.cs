using Microsoft.AspNetCore.SignalR;

namespace KPO_HW4.OrderService.OrdersFeature.SignalR;

public sealed class SignalROrderPushNotifier(IHubContext<OrdersHub> hub) : IOrderPushNotifier
{
    public Task StatusChanged(OrderStatusChangedPush payload, CancellationToken ct)
    {
        return hub.Clients.Groups(
                OrdersHub.UserGroup(payload.UserId),
                OrdersHub.OrderGroup(payload.OrderId))
            .SendAsync("OrderStatusChanged", payload, ct);
    }
}
