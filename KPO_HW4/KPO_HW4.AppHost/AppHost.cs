var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.KPO_HW4_Api>("api");

builder.AddProject<Projects.KPO_HW4_OrderService>("orderService");

builder.AddProject<Projects.KPO_HW4_PaymentsService>("kpo-hw4-paymentsservice");

builder.Build().Run();
