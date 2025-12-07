using KPO_HW3.Api.Infrastructure.External;

namespace KPO_HW3.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IFileStorageService, HttpFileStorageService>(client =>
        {
            client.BaseAddress = new Uri("https+http://file-storage");
        });

        services.AddHttpClient<IFileAnalysisService, HttpFileAnalysisService>(client =>
        {
            client.BaseAddress = new Uri("https+http://file-analysis");
        });


        return services;
    }
}