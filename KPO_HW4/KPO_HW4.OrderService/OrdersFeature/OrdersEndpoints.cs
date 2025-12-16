using KPO_HW4.OrderService.Infrastructure.Extensions;
using KPO_HW4.OrderService.OrdersFeature.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.OrdersFeature;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders")
            .WithTags("Orders");

        group.MapPost("/create", CreateOrderHandler)
            .WithDescription("Create order")
            .DisableAntiforgery();

        group.MapGet("/list/{userId}", ListOrdersHandler)
            .WithDescription("List orders");

        group.MapGet("/{orderId}", GetOrderStatus)
            .WithDescription("Get order status");


        group.MapHub<OrdersHub>("/ws/notifications/{userId}");

        return app;
    }

    private static async Task<Created<CreateOrderResponse>> CreateOrderHandler(
        [FromForm] CreateOrderRequest req,
        [FromServices] OrdersDbContext db,
        [FromServices] IPublishEndpoint publish,
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

        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/orders/{order.Id.Value}",
            new CreateOrderResponse(order.Id, order.Status));
    }

    private static async Task<Ok<List<OrderDto>>> ListOrdersHandler(
        [FromRoute] UserId userId,
        [FromServices] OrdersDbContext db, 
        CancellationToken ct)
    {
        var list = await db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(OrderDtoExtensions.CreateDto)
            .ToListAsync(ct);

        return TypedResults.Ok(list);
    }

    private static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderStatus(
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