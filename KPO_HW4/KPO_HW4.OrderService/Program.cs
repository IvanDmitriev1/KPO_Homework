using KPO_HW4.OrderService.Application;
using KPO_HW4.OrderService.Application.SignalR;
using KPO_HW4.OrderService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();
builder.AddServiceDefaults();

builder.Services.AddOpenApi("v1", openApi =>
{
    openApi.AddDocumentTransformer((document, _, _) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("redis") ??
                           throw new InvalidOperationException("Missing redis connection string"));

builder.Services.AddScoped<IOrderPushNotifier, SignalROrderPushNotifier>();

builder.Services.AddCors(o => o.AddPolicy("frontend", p =>
    p.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()));

var app = builder.Build();
app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Theme = ScalarTheme.Moon;
    options.Layout = ScalarLayout.Modern;
    options.WithDynamicBaseServerUrl();
});

app.UseCors("frontend");

app.MapOrdersEndpoints();

app.MapGet("/", () => TypedResults.Redirect("/scalar", true))
    .ExcludeFromApiReference();

app.Run();