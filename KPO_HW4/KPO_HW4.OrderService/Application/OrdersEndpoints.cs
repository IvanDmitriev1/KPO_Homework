using System.Runtime.CompilerServices;
using KPO_HW4.OrderService.Application.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.Application;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders")
            .WithTags("Orders");

        group.MapPost("/create", CreateOrderHandler)
            .WithDescription("Create order");

        group.MapGet("/list/{userId}", ListOrdersHandler)
            .WithDescription("List orders");

        group.MapGet("/status/{orderId}", GetOrderStatusHandler)
            .WithDescription("Get order status");


        group.MapHub<OrdersHub>("/hub/orderNotifications/{userId}");

        return app;
    }

    private static async Task<Created<CreateOrderResponse>> CreateOrderHandler(
        [FromBody] CreateOrderRequest req,
        [FromServices] OrdersDbContext db,
        [FromServices] IPublishEndpoint publish,
        [FromServices] IOrderPushNotifier notifier,
        CancellationToken ct)
    {
        var order = new Order()
        {
            UserId = req.UserId,
            AmountMinor = req.AmountMinor,
            Description = req.Description
        };

        db.Orders.Add(order);

        await publish.Publish(new PaymentRequested(
            order.Id,
            order.UserId,
            order.AmountMinor,
            DateTimeOffset.UtcNow), ct);

        await notifier.StatusChanged(
            new OrderStatusChangedPush(order.Id, order.UserId, order.Status), ct);

        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/orders/{order.Id.Value}",
            new CreateOrderResponse(order.Id, order.Status));
    }

    private static async IAsyncEnumerable<OrderDto> ListOrdersHandler(
        [FromRoute] UserId userId,
        [FromServices] OrdersDbContext db, 
        [EnumeratorCancellation] CancellationToken ct)
    {
        var query = db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(OrderDtoExtensions.CreateDto)
            .AsAsyncEnumerable()
            .WithCancellation(ct);

        await foreach (var order in query)
        {
            yield return order;
        }
    }

    private static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderStatusHandler(
        [FromRoute] OrderId orderId,
        [FromServices] OrdersDbContext db,
        CancellationToken ct)
    {
        var order = await db.Orders.AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(OrderDtoExtensions.CreateDto)
            .FirstOrDefaultAsync(ct);

        if (order is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(order);
    }
}