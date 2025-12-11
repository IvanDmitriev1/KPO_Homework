using Microsoft.EntityFrameworkCore;

namespace KPO_HW3.FileStorageService.Infrastructure.Data;

public class Migrator(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();

        await dbContext.Database.MigrateAsync(stoppingToken);
    }
}