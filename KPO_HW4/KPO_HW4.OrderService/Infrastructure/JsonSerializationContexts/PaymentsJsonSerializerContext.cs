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
[JsonSerializable(typeof(PaymentRequested))]
[JsonSerializable(typeof(PaymentSucceeded))]
[JsonSerializable(typeof(PaymentFailed))]
public partial class PaymentsJsonSerializerContext : JsonSerializerContext { }