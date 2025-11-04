using KPO_HW2.Data.Abstractions;
using KPO_HW2.Data.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace KPO_HW2.Data;

public static class ServicesExtensions
{
    public static IServiceCollection AddDb(this IServiceCollection services, string connectionStr)
    {
        services.AddMemoryCache();

        services.AddSingleton<IDbContextFactory<AppDbContext>>(sp =>
            new SqLiteDbContextFactory(connectionStr, sp.GetRequiredService<IMemoryCache>()));
        services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().Create());

        services.AddSingleton<IDbInitializer, SqLiteDbInitializer>();

        return services;
    }
}