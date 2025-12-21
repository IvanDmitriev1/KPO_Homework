using KPO_HW4.Ui.Infrastructure.JsonSerializationContexts;
using KPO_HW4.Ui.Infrastructure.Orders;
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

        var gatewayUrl = builder.Configuration["GATEWAY_HTTP"] ?? throw new InvalidOperationException("Gateway URL is not configured");
        var baseGatewayUrl = new Uri($"{gatewayUrl}/api/orders/");

        builder.Services.AddScoped<IOrdersRealtimeClient, OrdersRealtimeClient>(sp =>
            new OrdersRealtimeClient(baseGatewayUrl));

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(AccountsDtosSerializationContext.Default);
            chain.Add(OrdersJsonSerializerContext.Default);
        });
    }

   
}