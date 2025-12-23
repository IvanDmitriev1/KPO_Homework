using EntityFramework.Exceptions.PostgreSQL;
using KPO_HW4.OrderService.Application.Consumers;
using KPO_HW4.OrderService.Infrastructure.JsonSerializationContexts;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;

namespace KPO_HW4.OrderService.Infrastructure;

public static class ServicesExtensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<OrdersDbContext>("orders-db",
            settings => { },
            optionsBuilder =>
            {
                optionsBuilder.UseExceptionProcessor();
            });

        builder.Services.AddHostedService<Migrator>();
        builder.Services.AddPaymentsMessaging(builder.Configuration);

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(PaymentsJsonSerializerContext.Default);
            chain.Add(OrdersJsonSerializerContext.Default);
        });
    }

    private static void AddPaymentsMessaging(this IServiceCollection services, IConfiguration configuration) => services.AddMassTransit(c =>
    {
        c.AddConsumer<PaymentSucceededConsumer>();
        c.AddConsumer<PaymentFailedConsumer>();

        c.AddEntityFrameworkOutbox<OrdersDbContext>(o =>
        {
            o.UsePostgres();
            o.UseBusOutbox();

            o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
            o.QueryDelay = TimeSpan.FromSeconds(3);
        });

        c.AddConfigureEndpointsCallback((context, name, endpointCfg) =>
        {
            endpointCfg.UseEntityFrameworkOutbox<OrdersDbContext>(context);
        });

        var rabbitMqConnectionString = configuration.GetConnectionString("messaging");

        c.UsingRabbitMq((context, busCfg) =>
        {
            busCfg.Host(new Uri(rabbitMqConnectionString ?? throw new InvalidOperationException("rabbitMqConnectionString is null")));
            busCfg.ConfigureEndpoints(context);
        });
    });
}