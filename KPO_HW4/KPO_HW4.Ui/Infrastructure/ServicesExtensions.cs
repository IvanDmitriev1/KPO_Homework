using KPO_HW4.Ui.Infrastructure.JsonSerializationContexts;
using KPO_HW4.Ui.Infrastructure.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Json;

namespace KPO_HW4.Ui.Infrastructure;

public static class ServicesExtensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IOrdersApiClient, OrdersApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://gateway/api/orders/");
        });

        builder.Services.AddScoped<IOrdersRealtimeClient, OrdersRealtimeClient>(sp =>
        {
            var baseUri = sp.GetRequiredService<NavigationManager>().BaseUri;
            return new OrdersRealtimeClient(new Uri(baseUri));
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(AccountsDtosSerializationContext.Default);
            chain.Add(OrdersJsonSerializerContext.Default);
        });
    }

   
}