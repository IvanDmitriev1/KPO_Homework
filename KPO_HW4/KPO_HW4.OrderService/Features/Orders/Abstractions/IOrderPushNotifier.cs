using KPO_HW4.OrderService.Features.Orders.Models;

namespace KPO_HW4.OrderService.Features.Orders.Abstractions;

public interface IOrderPushNotifier
{
    Task StatusChanged(OrderStatusChangedPush push, CancellationToken ct);
}