using KPO_HW4.OrderService.Features.Orders;
using KPO_HW4.Shared.Contracts;
using System.Text.Json.Serialization;

namespace KPO_HW4.OrderService.Infrastructure.JsonSerializationContexts;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization,
    Converters = [
        typeof(UserId.UserIdSystemTextJsonConverter),
        typeof(AccountId.AccountIdSystemTextJsonConverter),
        typeof(OrderId.OrderIdSystemTextJsonConverter),
        typeof(PaymentTransactionId.PaymentTransactionIdSystemTextJsonConverter)])]
[JsonSerializable(typeof(CreateOrderRequest))]
[JsonSerializable(typeof(CreateOrderResponse))]
[JsonSerializable(typeof(List<OrderDto>))]
public partial class OrdersJsonSerializerContext : JsonSerializerContext { }