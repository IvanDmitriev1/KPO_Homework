using KPO_HW3.FileStorageService.Infrastructure.Data;

namespace KPO_HW3.FileStorageService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static TBuilder AddInfrastructure<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHostedService<Migrator>();

        return builder;
    }
}