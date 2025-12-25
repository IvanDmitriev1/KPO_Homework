using Microsoft.EntityFrameworkCore;

namespace KPO_HW4.OrderService.Data;

public class Migrator(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        await dbContext.Database.MigrateAsync(stoppingToken);
    }
}