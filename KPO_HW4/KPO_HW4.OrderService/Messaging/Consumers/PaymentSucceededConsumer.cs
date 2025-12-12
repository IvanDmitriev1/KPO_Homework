using KPO_HW4.OrderService.Data;
using KPO_HW4.Shared.Contracts.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.Messaging.Consumers;

public sealed class PaymentSucceededConsumer(OrdersDbContext db) : IConsumer<PaymentSucceeded>
{
    public async Task Consume(ConsumeContext<PaymentSucceeded> context)
    {
        var msg = context.Message;

        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == msg.OrderId, context.CancellationToken);

        if (order is null)
            return;

        order.MarkFinished();
        await db.SaveChangesAsync(context.CancellationToken);
    }
}