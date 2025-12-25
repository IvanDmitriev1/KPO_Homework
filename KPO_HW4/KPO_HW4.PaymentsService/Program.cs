using KPO_HW4.PaymentsService.Application;
using KPO_HW4.PaymentsService.Infrastructure;
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

var app = builder.Build();
app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Theme = ScalarTheme.Moon;
    options.Layout = ScalarLayout.Modern;
    options.WithDynamicBaseServerUrl();
});

app.MapAccountsEndpoints();

app.MapGet("/", () => TypedResults.Redirect("/scalar", true))
    .ExcludeFromApiReference();

app.Run();
