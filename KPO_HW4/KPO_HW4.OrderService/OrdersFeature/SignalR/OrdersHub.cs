using Microsoft.AspNetCore.SignalR;

namespace KPO_HW4.OrderService.OrdersFeature.SignalR;

public sealed class OrdersHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var raw = http?.Request.Query["userId"].FirstOrDefault();

        if (UserId.TryParse(raw, null, out var userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
            return;
        }

        Context.Abort();
    }

    public Task SubscribeOrder(OrderId orderId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, OrderGroup(orderId));

    public Task UnsubscribeOrder(OrderId orderId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, OrderGroup(orderId));

    public static string UserGroup(UserId userId) => $"user:{userId}";
    public static string OrderGroup(OrderId orderId) => $"order:{orderId}";
}
