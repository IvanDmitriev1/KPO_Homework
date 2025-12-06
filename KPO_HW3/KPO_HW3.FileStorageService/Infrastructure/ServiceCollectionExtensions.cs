using KPO_HW3.FileStorageService.Infrastructure.Data;
using KPO_HW3.FileStorageService.Infrastructure.FileStorage;
using Minio;

namespace KPO_HW3.FileStorageService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static TBuilder AddInfrastructure<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddNpgsqlDbContext<FileStorageDbContext>("file-storage-db");
        builder.AddMinioClient("minio");

        builder.Services.AddHostedService<Migrator>();

        var c = builder.Configuration;

        builder.Services.AddSingleton<IFileStorage>(sp =>
            new MinioFileStorage(
                sp.GetRequiredService<IMinioClient>(),
                "files"));

        return builder;
    }
}