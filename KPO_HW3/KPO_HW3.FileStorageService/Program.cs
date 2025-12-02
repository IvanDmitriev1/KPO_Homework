using KPO_HW3.FileStorageService.Application.Services;
using KPO_HW3.FileStorageService.Endpoints;
using KPO_HW3.FileStorageService.Infrastructure;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<FileStorageDbContext>("file-storage-db");

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IWorkService, WorkService>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();

app.MapWorkEndpoints();

app.Run();
