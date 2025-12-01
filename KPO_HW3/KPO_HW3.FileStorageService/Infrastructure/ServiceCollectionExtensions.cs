using FluentValidation;
using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Infrastructure.FileStorage;
using System;

namespace KPO_HW3.FileStorageService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<Migrator>();

        services.AddSingleton<IFileStorage>(sp =>
            new LocalFileStorage(
                sp.GetRequiredService<IHostEnvironment>(),
                configuration["FileStorage:RootPath"]));

        services.AddScoped<IValidator<Work>, WorkValidator>();

        return services;
    }
}