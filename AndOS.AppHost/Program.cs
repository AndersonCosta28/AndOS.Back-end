var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("pg", port: 5432)
#if DEBUG
    .WithDataVolume()
#endif
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var api = builder.AddProject<Projects.AndOS_API>("andos-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WithReplicas(1);

#if DEBUG
builder.AddAzureStorage("storage")
            .RunAsEmulator(config => config.WithImageTag("latest")
                .WithBlobPort(10000)
                .WithQueuePort(10001)
                .WithTablePort(10002))
            .AddBlobs("blob");
#endif

builder.Build().Run();
