namespace KPO_HW4.OrderService.OrdersFeature.Abstractions;

public interface IOrderPushNotifier
{
    Task StatusChanged(OrderStatusChangedPush push, CancellationToken ct);
}