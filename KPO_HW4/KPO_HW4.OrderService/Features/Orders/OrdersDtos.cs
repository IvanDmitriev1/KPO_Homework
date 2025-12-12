using System.ComponentModel.DataAnnotations;
using KPO_HW4.OrderService.Data.Entities;
using KPO_HW4.Shared.Contracts;

namespace KPO_HW4.OrderService.Features.Orders;

public sealed record CreateOrderRequest(UserId UserId, [Range(1, 100000000)] long AmountMinor, string Description);
public sealed record CreateOrderResponse(OrderId OrderId, OrderStatus Status);

public sealed record OrderDto(
    OrderId OrderId,
    decimal Amount,
    string Description,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
