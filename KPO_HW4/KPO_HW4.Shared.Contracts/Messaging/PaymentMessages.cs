using KPO_HW4.Shared.Contracts.Common;

namespace KPO_HW4.Shared.Contracts.Messaging;

public sealed record PaymentRequested(
    OrderId OrderId,
    UserId UserId,
    Int64 AmountMinor,
    DateTimeOffset OccurredAt);

public sealed record PaymentSucceeded(
    OrderId OrderId,
    UserId UserId,
    Int64 AmountMinor,
    DateTimeOffset OccurredAt);

public sealed record PaymentFailed(
    OrderId OrderId,
    UserId UserId,
    Int64 AmountMinor,
    PaymentFailureCode FailureCode,
    DateTimeOffset OccurredAt);
