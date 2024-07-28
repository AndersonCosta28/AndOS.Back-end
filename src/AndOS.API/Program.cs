global using MediatR;
global using AndOS.Core.Enums;
global using File = AndOS.Domain.Entities.File;
global using AndOS.Application.Interfaces;
global using AndOS.Domain.Entities;
global using AndOS.Domain.Interfaces;
global using AndOS.Shared.DTOs;
using AndOS.API.Cors;
using AndOS.API.Localization;
using AndOS.API.Mapper;
using AndOS.Infrastructure;
using AndOS.Infrastructure.Database;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AndOS.API;

public class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        configBuild(builder);
        builder.EnrichNpgsqlDbContext<AppDbContext>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        configApp(app);
        app.Run();
    }

    private static void configApp(WebApplication app)
    {
        app.UseCors("*");

        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
        ArgumentNullException.ThrowIfNull(localizationOptions);
        app.UseRequestLocalization(localizationOptions);

        app.MapSwagger().RequireAuthorization();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseExceptionHandler();
    }

    private static void configBuild(WebApplicationBuilder builder)
    {
        ConfigurationManager configuration = builder.Configuration;

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.Startup).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddLocalizationService();
        builder.Services.AddMapperService();
        builder.Services.AddCorsService(configuration);
        builder.Services.AddControllers()
            .AddDataAnnotationsLocalization();
        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddInfrastructure(configuration);

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AndOS.API", Version = "v1" });
            c.CustomSchemaIds(type => type.FullName);
        });
    }
}