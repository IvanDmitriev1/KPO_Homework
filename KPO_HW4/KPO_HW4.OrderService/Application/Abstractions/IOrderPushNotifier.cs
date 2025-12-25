namespace KPO_HW4.OrderService.Application.Abstractions;

public interface IOrderPushNotifier
{
    Task StatusChanged(OrderStatusChangedPush push, CancellationToken ct);
}