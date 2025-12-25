using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.PaymentsService.Data;

public class Migrator(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();

        await dbContext.Database.MigrateAsync(stoppingToken);
    }
}