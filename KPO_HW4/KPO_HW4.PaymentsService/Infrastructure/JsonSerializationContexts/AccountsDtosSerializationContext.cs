using System.Text.Json.Serialization;

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
[JsonSerializable(typeof(CreateAccountResponse))]
[JsonSerializable(typeof(TopUpAccountRequest))]
[JsonSerializable(typeof(AccountBalanceResponse))]
[JsonSerializable(typeof(PaymentTransactionDto))]
public partial class AccountsDtosSerializationContext : JsonSerializerContext { }