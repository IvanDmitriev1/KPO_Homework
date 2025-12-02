using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileAnalysisService.Infrastructure.Data;

public class Migrator(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();

        await dbContext.Database.MigrateAsync(stoppingToken);
    }
}