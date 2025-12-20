using KPO_HW4.OrderService.Infrastructure;
using KPO_HW4.OrderService.OrdersFeature;
using KPO_HW4.OrderService.OrdersFeature.SignalR;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddSignalR();
builder.Services.AddScoped<IOrderPushNotifier, SignalROrderPushNotifier>();

builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()));

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

app.UseCors("frontend");

app.MapOrdersEndpoints();

app.MapGet("/", () => TypedResults.Redirect("/scalar", true));

app.Run();