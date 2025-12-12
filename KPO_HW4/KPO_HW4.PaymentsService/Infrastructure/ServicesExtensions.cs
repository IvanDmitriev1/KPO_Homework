using KPO_HW4.PaymentsService.Data;
using KPO_HW4.PaymentsService.Infrastructure.JsonSerializationContexts;
using KPO_HW4.PaymentsService.Messaging.Consumers;
using KPO_HW4.Shared.Contracts;
using MassTransit;

namespace KPO_HW4.PaymentsService.Infrastructure;

public static class ServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddHostedService<Migrator>();
            services.AddPaymentsMessaging(configuration);

            services.AddControllers().AddJsonOptions(options =>
            {
                var chain = options.JsonSerializerOptions.TypeInfoResolverChain;

                chain.Add(ContractsJsonSerializerContext.Default);
                chain.Add(AccountsDtosSerializationContext.Default);
            });

            return services;
        }

        private void AddPaymentsMessaging(IConfiguration configuration) => services.AddMassTransit(c =>
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
                busCfg.Host(new Uri(rabbitMqConnectionString ?? throw new InvalidOperationException("rabbitMqConnectionString is null")));
                busCfg.ConfigureEndpoints(context);
            });
        });
    }
}