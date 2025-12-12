using KPO_HW4.OrderService.Data.Entities;
using KPO_HW4.OrderService.Features.Orders;
using System.Linq.Expressions;

namespace KPO_HW4.OrderService.Infrastructure.Extensions;

public static class OrderDtoExtensions
{
    public static readonly Expression<Func<Order, OrderDto>> CreateDto = static o => new OrderDto(
        OrderId: o.Id,
        Amount: o.AmountMinor / 100m,
        Description: o.Description,
        Status: o.Status,
        CreatedAt: o.CreatedAt,
        UpdatedAt: o.UpdatedAt);
}