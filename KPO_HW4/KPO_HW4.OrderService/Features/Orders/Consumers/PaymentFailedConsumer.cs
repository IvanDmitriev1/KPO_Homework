using KPO_HW4.OrderService.Data;
using KPO_HW4.Shared.Contracts.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.Features.Orders.Consumers;

public sealed class PaymentFailedConsumer(OrdersDbContext db) : IConsumer<PaymentFailed>
{
    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        var msg = context.Message;

        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == msg.OrderId, context.CancellationToken);

        if (order is null)
            return;

        order.MarkCancelled();
        await db.SaveChangesAsync(context.CancellationToken);
    }
}