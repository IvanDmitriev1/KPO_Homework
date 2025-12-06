using KPO_HW3.FileAnalysisService.Endpoints;
using KPO_HW3.FileAnalysisService.Infrastructure;
using KPO_HW3.FileAnalysisService.Infrastructure.Data;
using KPO_HW3.FileAnalysisService.Services;
using KPO_HW3.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<AnalysisDbContext>("file-analysis-db");

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAnalysisService, AnalysisService>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();

app.MapReportEndpoints();

app.Run();
