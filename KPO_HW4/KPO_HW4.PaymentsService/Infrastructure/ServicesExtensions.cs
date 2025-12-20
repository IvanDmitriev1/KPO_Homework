using EntityFramework.Exceptions.PostgreSQL;
using KPO_HW4.PaymentsService.AccountsFeature.Consumers;
using KPO_HW4.PaymentsService.Infrastructure.JsonSerializationContexts;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;

namespace KPO_HW4.PaymentsService.Infrastructure;

public static class ServicesExtensions
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<PaymentsDbContext>("payments-db",
            settings => { },
            optionsBuilder => { optionsBuilder.UseExceptionProcessor(); });

        builder.Services.AddHostedService<Migrator>();
        builder.Services.AddPaymentsMessaging(builder.Configuration);

        builder.Services.Configure<JsonOptions>(options =>
        {
            var chain = options.SerializerOptions.TypeInfoResolverChain;
            chain.Add(ContractsJsonSerializerContext.Default);
            chain.Add(AccountsDtosSerializationContext.Default);
        });
    }

    private static void AddPaymentsMessaging(this IServiceCollection services, IConfiguration configuration) =>
        services.AddMassTransit(c =>
        {
            c.AddConsumer<PaymentRequestedConsumer>(cfg =>
            {
                cfg.UseMessageRetry(r => r.Intervals(100, 300, 1000, 2000));
            });

            c.AddEntityFrameworkOutbox<PaymentsDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();

                o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
                o.QueryDelay = TimeSpan.FromSeconds(1);
            });

            c.AddConfigureEndpointsCallback((context, name, endpointCfg) =>
            {
                endpointCfg.UseEntityFrameworkOutbox<PaymentsDbContext>(context);
            });

            var rabbitMqConnectionString = configuration.GetConnectionString("messaging");

            c.UsingRabbitMq((context, busCfg) =>
            {
                busCfg.Host(new Uri(rabbitMqConnectionString ??
                                    throw new InvalidOperationException("rabbitMqConnectionString is null")));
                busCfg.ConfigureEndpoints(context);
            });
        });
}