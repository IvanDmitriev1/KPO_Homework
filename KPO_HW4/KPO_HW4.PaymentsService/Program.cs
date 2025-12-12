using EntityFramework.Exceptions.PostgreSQL;
using KPO_HW4.PaymentsService.Data;
using KPO_HW4.PaymentsService.Features.Accounts;
using KPO_HW4.PaymentsService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<PaymentsDbContext>("payments-db",
    settings => {},
    optionsBuilder =>
{
    optionsBuilder.UseExceptionProcessor();
});


builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

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
