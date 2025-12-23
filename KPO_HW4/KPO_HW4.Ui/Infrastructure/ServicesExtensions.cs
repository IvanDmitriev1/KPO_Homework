using KPO_HW4.Ui.Infrastructure.Accounts;
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

        builder.Services.AddHttpClient<IAccountsApi, AccountsApi>(client =>
        {
            client.BaseAddress = new Uri("https+http://gateway/api/accounts/");
        });

        var baseUri = new Uri(builder.Configuration["GATEWAY_HTTPS"] ??
                              builder.Configuration["GATEWAY_HTTP"] ??
                              throw new InvalidOperationException("'GATEWAY_HTTP' not configured"));

        builder.Services.AddScoped<IOrdersRealtimeClient, OrdersRealtimeClient>(sp =>
            new OrdersRealtimeClient(baseUri));

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(AccountsSerializationContext.Default);
            chain.Add(OrdersJsonSerializerContext.Default);
        });
    }

   
}