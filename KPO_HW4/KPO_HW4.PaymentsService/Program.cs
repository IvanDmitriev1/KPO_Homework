using KPO_HW4.PaymentsService.AccountsFeature;
using KPO_HW4.PaymentsService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

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

app.MapAccountsEndpoints();

app.MapGet("/", () => TypedResults.Redirect("/scalar", true));

app.Run();
