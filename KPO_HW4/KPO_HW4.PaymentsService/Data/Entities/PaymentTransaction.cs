namespace KPO_HW4.PaymentsService.Data.Entities;

public sealed class PaymentTransaction
{
    public PaymentTransactionId Id { get; init; }

    public required AccountId AccountId { get; init; }

    /// <summary>
    /// Для Debit: OrderId
    /// Для TopUp: генерируем
    /// </summary>
    public required Guid ReferenceId { get; init; }
    public required PaymentTransactionType Type { get; init; }
    public required Int64 AmountMinor { get; init; }

    public PaymentTransactionStatus Status { get; private set; } = PaymentTransactionStatus.Processing;
    public PaymentFailureCode FailureCode { get; private set; } = PaymentFailureCode.None;

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void MarkSucceeded()
    {
        Status = PaymentTransactionStatus.Succeeded;
        FailureCode = PaymentFailureCode.None;
    }
    public void MarkFailed(PaymentFailureCode code)
    {
        Status = PaymentTransactionStatus.Failed;
        FailureCode = code;
    }

    public static PaymentTransaction CreateTopUp(AccountId accountId, Int64 amountMinor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountMinor);

        return new PaymentTransaction
        {
            AccountId = accountId,
            Type = PaymentTransactionType.TopUp,
            ReferenceId = Guid.CreateVersion7(),
            AmountMinor = amountMinor
        };
    }

    public static PaymentTransaction CreateDebit(AccountId accountId, OrderId orderId, Int64 amountMinor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountMinor);

        return new PaymentTransaction
        {
            AccountId = accountId,
            ReferenceId = orderId.Value,
            Type = PaymentTransactionType.Debit,
            AmountMinor = amountMinor
        };
    }
}