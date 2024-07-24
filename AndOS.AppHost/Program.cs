var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("pg")
                                                          .AddDatabase("postgresdb");

var api = builder.AddProject<Projects.AndOS_API>("andos-api")
                                            .WithExternalHttpEndpoints()
                                            .WithReference(postgresdb);
builder.Build().Run();
