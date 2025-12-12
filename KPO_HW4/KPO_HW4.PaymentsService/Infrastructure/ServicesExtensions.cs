using EntityFramework.Exceptions.PostgreSQL;
using KPO_HW4.PaymentsService.Data;
using KPO_HW4.PaymentsService.Infrastructure.JsonSerializationContexts;
using KPO_HW4.PaymentsService.Messaging.Consumers;
using MassTransit;

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

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var chain = options.JsonSerializerOptions.TypeInfoResolverChain;

            chain.Add(ContractsJsonSerializerContext.Default);
        });
    }

    private static void AddPaymentsMessaging(this IServiceCollection services, IConfiguration configuration) =>
        services.AddMassTransit(c =>
        {
            c.AddConsumer<PaymentRequestedConsumer>();

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