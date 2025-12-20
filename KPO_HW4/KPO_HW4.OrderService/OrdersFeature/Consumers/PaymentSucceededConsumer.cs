using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.OrdersFeature.Consumers;

public sealed class PaymentSucceededConsumer(OrdersDbContext db, IOrderPushNotifier pushNotifier) : IConsumer<PaymentSucceeded>
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

        await pushNotifier.StatusChanged(
            new OrderStatusChangedPush(order.Id, order.UserId, order.Status),
            context.CancellationToken);
    }
}