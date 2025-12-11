using KPO_HW3.FileStorageService.Endpoints;
using KPO_HW3.FileStorageService.Infrastructure;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Services;
using KPO_HW3.ServiceDefaults;
using Minio;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<FileStorageDbContext>("file-storage-db");
builder.AddMinioClient("minio");

builder.AddInfrastructure();
builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IWorkSubmissionService, WorkSubmissionService>();

builder.Services.AddSingleton<IFileStorage>(sp =>
    new MinioFileStorage(
        sp.GetRequiredService<IMinioClient>(),
        "files"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapWorkEndpoints();

app.Run();
