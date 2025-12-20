using KPO_HW4.Shared.Contracts.Common;

namespace KPO_HW4.Shared.Contracts.Order;

public sealed record OrderStatusChangedPush(
    OrderId OrderId,
    UserId UserId,
    OrderStatus Status)
{
    public static string Name = "OrderStatusChanged";
}