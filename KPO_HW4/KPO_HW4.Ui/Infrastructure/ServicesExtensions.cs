using KPO_HW4.Ui.Abstractions.Notifications;
using KPO_HW4.Ui.Infrastructure.JsonSerializationContexts;
using KPO_HW4.Ui.Infrastructure.Notifications;
using Microsoft.AspNetCore.Http.Json;

namespace KPO_HW4.Ui.Infrastructure;

public static class ServicesExtensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<INotifyService, NotifyJsModule>();

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(AccountsDtosSerializationContext.Default);
            chain.Add(OrdersJsonSerializerContext.Default);
        });
    }

   
}