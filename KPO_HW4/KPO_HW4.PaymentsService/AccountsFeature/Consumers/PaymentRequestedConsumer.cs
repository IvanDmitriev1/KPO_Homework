using KPO_HW4.PaymentsService.Data.Entities;
using KPO_HW4.Shared.Contracts.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.PaymentsService.AccountsFeature.Consumers;

public class PaymentRequestedConsumer(PaymentsDbContext db) : IConsumer<PaymentRequested>
{
    public async Task Consume(ConsumeContext<PaymentRequested> context)
    {
        var msg = context.Message;

        var accountId = await db.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == msg.UserId)
            .Select(a => new { a.Id })
            .FirstOrDefaultAsync(context.CancellationToken);

        if (accountId is null)
        {
            await context.Publish(new PaymentFailed(
                msg.OrderId,
                msg.UserId,
                msg.AmountMinor,
                PaymentFailureCode.AccountNotFound,
                DateTimeOffset.UtcNow));

            await db.SaveChangesAsync(context.CancellationToken);
            return;
        }

        var existing = await db.Transactions
            .Where(t => t.Type == PaymentTransactionType.Debit
                        && t.ReferenceId == msg.OrderId.Value)
            .SingleOrDefaultAsync(context.CancellationToken);

        if (existing is not null && existing.Status != PaymentTransactionStatus.Processing)
        {
            await PublishFromExisting(context, msg, existing, DateTimeOffset.UtcNow, context.CancellationToken);
            await db.SaveChangesAsync(context.CancellationToken);
            return;
        }

        var paymentTransaction = PaymentTransaction.CreateDebit(accountId.Id, msg.OrderId, msg.AmountMinor);
        db.Transactions.Add(paymentTransaction);

        var affected = await db.Accounts
            .Where(a => a.Id == accountId.Id && a.BalanceMinor >= msg.AmountMinor)
            .ExecuteUpdateAsync(s => s.SetProperty(
                    a => a.BalanceMinor,
                    a => a.BalanceMinor - msg.AmountMinor),
                context.CancellationToken);

        if (affected == 1)
        {
            paymentTransaction.MarkSucceeded();

            await context.Publish(new PaymentSucceeded(
                msg.OrderId, msg.UserId, msg.AmountMinor, DateTimeOffset.UtcNow));
        }
        else
        {
            paymentTransaction.MarkFailed(PaymentFailureCode.InsufficientFunds);

            await context.Publish(new PaymentFailed(
                msg.OrderId, msg.UserId, msg.AmountMinor,
                PaymentFailureCode.InsufficientFunds,
                DateTimeOffset.UtcNow));
        }

        await db.SaveChangesAsync(context.CancellationToken);
    }

    private static Task PublishFromExisting(
        ConsumeContext<PaymentRequested> context,
        PaymentRequested msg,
        PaymentTransaction tx,
        DateTimeOffset now,
        CancellationToken ct)
    {
        return tx.Status switch
        {
            PaymentTransactionStatus.Succeeded =>
                context.Publish(new PaymentSucceeded(msg.OrderId, msg.UserId, msg.AmountMinor, now), ct),

            PaymentTransactionStatus.Failed =>
                context.Publish(new PaymentFailed(
                    msg.OrderId, msg.UserId, msg.AmountMinor,
                    tx.FailureCode == PaymentFailureCode.None ? PaymentFailureCode.Unknown : tx.FailureCode,
                    now), ct),

            _ =>
                context.Publish(new PaymentFailed(
                    msg.OrderId, msg.UserId, msg.AmountMinor,
                    PaymentFailureCode.Unknown,
                    now), ct)
        };
    }
}