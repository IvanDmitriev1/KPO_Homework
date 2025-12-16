using System.ComponentModel.DataAnnotations;

namespace KPO_HW4.OrderService.OrdersFeature.Models;

public sealed record CreateOrderRequest(UserId UserId, [Range(1, 100000000)] long AmountMinor, string Description);
public sealed record CreateOrderResponse(OrderId OrderId, OrderStatus Status);

public sealed record OrderDto(
    OrderId OrderId,
    decimal Amount,
    string Description,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record OrderStatusChangedPush(
    OrderId OrderId,
    UserId UserId,
    OrderStatus Status);