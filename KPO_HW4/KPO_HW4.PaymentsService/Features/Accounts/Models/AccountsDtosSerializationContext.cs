using System.Text.Json.Serialization;

namespace KPO_HW4.PaymentsService.Features.Accounts.Models;

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
[JsonSerializable(typeof(TopUpRequest))]
[JsonSerializable(typeof(BalanceResponse))]
public partial class AccountsDtosSerializationContext : JsonSerializerContext { }