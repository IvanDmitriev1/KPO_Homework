using System.Net;

namespace KPO_HW4.Ui.Infrastructure.Orders;

public sealed class OrdersApiClient(HttpClient client) : IOrdersApiClient
{
    public async Task<IReadOnlyList<OrderDto>> ListByUserAsync(UserId userId, CancellationToken ct = default)
    {
        var url = $"/{userId.Value:D}";

        var list = await client.GetFromJsonAsync<List<OrderDto>>(url, ct);
        return list ?? [];
    }

    public async Task<OrderDto?> GetAsync(OrderId orderId, CancellationToken ct = default)
    {
        var url = $"/status/{orderId.Value:D}";

        using var resp = await client.GetAsync(url, ct);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: ct);
    }
}
