namespace KPO_HW4.Ui.Abstractions;

public interface IOrdersApiClient
{
    Task<CreateOrderResponse> CreateAsync(CreateOrderRequest req, CancellationToken ct = default);
    Task<List<OrderDto>> ListByUserAsync(UserId userId, CancellationToken ct = default);
    Task<OrderDto?> GetAsync(OrderId orderId, CancellationToken ct = default);
}