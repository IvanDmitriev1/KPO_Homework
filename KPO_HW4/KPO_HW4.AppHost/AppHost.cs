using Aspire.Hosting.Yarp;
using Aspire.Hosting.Yarp.Transforms;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var env = builder.AddDockerComposeEnvironment("docker")
    .WithProperties(env =>
    {
        env.DefaultNetworkName = "my-network";
    })
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(18888);
        dashboard.WithForwardedHeaders();
    });

var redis = builder.AddRedis("redis")
    .WithRedisInsight();

var rabbitmq = builder
    .AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithDataVolume()
    .PublishAsDockerComposeService((resource, service) => {});

var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .PublishAsDockerComposeService((resource, service) => { });

var ordersDb = postgres.AddDatabase("orders-db");
var paymentsDb = postgres.AddDatabase("payments-db");

var paymentsService = builder
    .AddProject<Projects.KPO_HW4_PaymentsService>("paymentsService")
    .WithReference(rabbitmq)
    .WithReference(paymentsDb)
    .WaitFor(paymentsDb)
    .WaitFor(rabbitmq)
    .PublishAsDockerComposeService((resource, service) => { });

var ordersService = builder.AddProject<Projects.KPO_HW4_OrderService>("orderService")
    .WithReference(rabbitmq)
    .WithReference(ordersDb)
    .WithReference(redis)
    .WaitFor(ordersDb)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WithReplicas(2)
    .PublishAsDockerComposeService((resource, service) => { });

var ui = builder.AddProject<Projects.KPO_HW4_Ui>("ui")
    .PublishAsDockerComposeService((resource, service) => { });

var gateway = builder.AddYarp("gateway")
    .WithHostPort(8080)
    .WithHostHttpsPort(8433)
    .WithConfiguration(yarp =>
    {
        //Orders
        yarp.AddRoute("/api/orders/scalar/{**catchAll}", ordersService)
            .WithTransformPathRemovePrefix("/api/orders/scalar")
            .WithTransformPathPrefix("/scalar");

        yarp.AddRoute("/api/orders/openapi/{**catchAll}", ordersService)
            .WithTransformPathRemovePrefix("/api/orders");

        yarp.AddRoute("/api/orders/{**catch-all}", ordersService)
            .WithTransformPathRemovePrefix("/api");


        //Payments
        yarp.AddRoute("/api/accounts/scalar/{**catchAll}", paymentsService)
            .WithTransformPathRemovePrefix("/api/accounts/scalar")
            .WithTransformPathPrefix("/scalar");

        yarp.AddRoute("/api/accounts/openapi/{**catchAll}", paymentsService)
            .WithTransformPathRemovePrefix("/api/accounts");

        yarp.AddRoute("/api/accounts/{**catch-all}", paymentsService)
            .WithTransformPathRemovePrefix("/api");


        //UI
        yarp.AddRoute("/{**catch-all}", ui);
    })
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Ports.Add("8080:5000");
    });

ui.WithReference(gateway)
    .WaitFor(gateway);

if (builder.Environment.IsDevelopment())
{
    var tunnel = builder.AddDevTunnel("mytunnel")
        .WithReference(gateway)
        .WithAnonymousAccess();
}

builder.Build().Run();
