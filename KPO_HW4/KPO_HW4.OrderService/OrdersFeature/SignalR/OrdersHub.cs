using MassTransit.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace KPO_HW4.OrderService.OrdersFeature.SignalR;

public sealed class OrdersHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();

        if (http is null ||
            !http.Request.RouteValues.TryGetValue("userId", out string raw) ||
            !UserId.TryParse(raw, null, out var userId))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
    }

    public static string UserGroup(UserId userId) => $"user:{userId}";
}
