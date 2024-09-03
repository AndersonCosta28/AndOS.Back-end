using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("pg", port: 5432)
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("postgresdb");

var api = builder.AddProject<Projects.AndOS_API>("andos-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb);

if (builder.Environment.IsDevelopment())
{
    builder.AddAzureStorage("storage")
            .RunAsEmulator(config => config.WithImageTag("latest")
                .WithBlobPort(10000)
                .WithQueuePort(10001)
                .WithTablePort(10002))
            .AddBlobs("blob");
}

builder.Build().Run();
