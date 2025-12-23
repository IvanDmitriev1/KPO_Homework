using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.Application.Consumers;

public sealed class PaymentFailedConsumer(OrdersDbContext db, IOrderPushNotifier notifier) : IConsumer<PaymentFailed>
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

        await notifier.StatusChanged(
            new OrderStatusChangedPush(order.Id, order.UserId, order.Status),
            context.CancellationToken);
    }
}