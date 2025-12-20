namespace KPO_HW4.Ui.Abstractions;

public interface IOrdersApiClient
{
    Task<IReadOnlyList<OrderDto>> ListByUserAsync(UserId userId, CancellationToken ct = default);
    Task<OrderDto?> GetAsync(OrderId orderId, CancellationToken ct = default);
}