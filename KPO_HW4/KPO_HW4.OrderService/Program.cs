using KPO_HW4.OrderService.Features.Orders;
using KPO_HW4.OrderService.Features.Orders.Abstractions;
using KPO_HW4.OrderService.Features.Orders.SignalR;
using KPO_HW4.OrderService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddSignalR(options =>
{
    
});

builder.Services.AddScoped<IOrderPushNotifier, SignalROrderPushNotifier>();

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Moon;
        options.Layout = ScalarLayout.Modern;
        options.WithDynamicBaseServerUrl();
    });
}

app.MapOrdersEndpoints();

app.MapGet("/", () => TypedResults.Redirect("/scalar", true));

app.Run();