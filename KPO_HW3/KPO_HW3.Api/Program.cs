using KPO_HW3.Api.Abstractions;
using KPO_HW3.Api.Endpoints;
using KPO_HW3.Api.Infrastructure;
using KPO_HW3.Api.Services;
using KPO_HW3.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();

builder.Services.AddScoped<WorkApiServices>();

builder.Services.AddHttpClient<IWordCloudService, QuickChartWordCloudService>(client =>
{
    client.BaseAddress = new Uri("https://quickchart.io");
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

app.UseHttpsRedirection();

app.MapWorkEndpoints();
app.MapWordCloudEndpoints();
app.MapGet("/", () => Results.Redirect("/scalar", true));

app.Run();
