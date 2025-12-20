using System.ComponentModel.DataAnnotations;
using KPO_HW4.Shared.Contracts.Common;

namespace KPO_HW4.Shared.Contracts.Order;

public sealed record CreateOrderRequest(UserId UserId, [Range(1, 100000000)] long AmountMinor, string Description);
public sealed record CreateOrderResponse(OrderId OrderId, OrderStatus Status);

public sealed record OrderStatusChangedPush(
    OrderId OrderId,
    UserId UserId,
    OrderStatus Status);
