using KPO_HW4.Shared.Contracts.Common;

namespace KPO_HW4.Shared.Contracts.Account;

public sealed record PaymentTransactionDto(
    UserId UserId,
    Guid ReferenceId,
    PaymentTransactionType Type,
    decimal Amount,
    PaymentTransactionStatus Status,
    PaymentFailureCode FailureCode,
    DateTimeOffset CreatedAt);