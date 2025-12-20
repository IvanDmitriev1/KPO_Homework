using KPO_HW4.Shared.Contracts.Common;

namespace KPO_HW4.Shared.Contracts.Order;

public sealed record OrderDto(
    OrderId OrderId,
    decimal Amount,
    string Description,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);