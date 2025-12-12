using System.Text.Json.Serialization;
using KPO_HW4.Shared.Contracts;
using KPO_HW4.Shared.Contracts.Messaging;

namespace KPO_HW4.PaymentsService.Infrastructure.JsonSerializationContexts;

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
public partial class ContractsJsonSerializerContext : JsonSerializerContext { }