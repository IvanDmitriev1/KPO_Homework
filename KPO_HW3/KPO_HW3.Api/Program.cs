using KPO_HW3.Api.Endpoints;
using KPO_HW3.Api.Services;
using KPO_HW3.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();


builder.Services.AddHttpClient<IFileStorageService, HttpFileStorageService>(client =>
{
    client.BaseAddress = new Uri("https+http://file-storage");
});

builder.Services.AddHttpClient<IFileAnalysisService, HttpFileAnalysisService>(client =>
{
    client.BaseAddress = new Uri("https+http://file-analysis");
});

builder.Services.AddScoped<WorkApiServices>();

var app = builder.Build();
app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.ShowDeveloperTools = DeveloperToolsVisibility.Always;

    if (app.Environment.IsDevelopment())
    {
        options
            .WithTitle("Test1")
            .AddServer("https://g0fhb46x-7140.euw.devtunnels.ms")
            .AddServer("https://localhost:7140", "Local");
    }
    else
    {
        options.WithDynamicBaseServerUrl();
    }
});

app.UseHttpsRedirection();

app.MapWorkEndpoints();
app.MapGet("/", () => Results.Redirect("/scalar", true));

app.Run();
