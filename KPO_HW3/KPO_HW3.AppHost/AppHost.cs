using Aspire.Hosting;
using Aspire.Hosting.Docker.Resources.ServiceNodes;

var builder = DistributedApplication.CreateBuilder(args);

var env = builder.AddDockerComposeEnvironment("docker")
    .WithProperties(env =>
    {
        env.DefaultNetworkName = "my-network";
        env.RequiresImageBuildAndPush();
    })
    .WithDashboard(dashboard =>
    {
        dashboard.WithHostPort(18888);
        dashboard.WithForwardedHeaders(true);
    });

var postgres = builder.AddPostgres("db")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithComputeEnvironment(env);

var fileStorageDb = postgres.AddDatabase("file-storage-db");
var fileAnalysisDb = postgres.AddDatabase("file-analysis-db");

var fileStorageService = builder.AddProject<Projects.KPO_HW3_FileStorageService>("file-storage")
    .WithReference(fileStorageDb)
    .WaitFor(fileStorageDb)
    .PublishAsDockerComposeService((resource, service) =>
    {
        service.Volumes.Add(new Volume
        {
            Name = "file-storage-data",
            Target = "/app/data",
            Type = "volume",
            ReadOnly = false
        });
    });

var fileAnalysis = builder.AddProject<Projects.KPO_HW3_FileAnalysisService>("file-analysis")
    .WithReference(fileAnalysisDb)
    .WaitFor(fileAnalysisDb)
    .WaitFor(fileStorageService)
    .PublishAsDockerComposeService((resource, service) =>
    {
        
    });

builder.AddProject<Projects.KPO_HW3_Api>("api")
    .WithReference(fileStorageService)
    .WithReference(fileAnalysis)
    .WaitFor(fileAnalysis)
    .PublishAsDockerComposeService((resource, service) =>
    {
        
    });

builder.Build().Run();
