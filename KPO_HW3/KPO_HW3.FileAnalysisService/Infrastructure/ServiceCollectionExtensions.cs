using KPO_HW3.FileAnalysisService.Infrastructure.Data;
using KPO_HW3.FileAnalysisService.Services;

namespace KPO_HW3.FileAnalysisService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<Migrator>();

        services.AddHttpClient<IFileStorageApi, HttpFileStorageApi>(client =>
            {
                const string baseUrl = "https+http://file-storage";
                client.BaseAddress = new Uri(baseUrl);
            });

        return services;
    }
}