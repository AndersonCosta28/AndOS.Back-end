global using AndOS.Application.Interfaces;
global using AndOS.Core.Constants;
global using AndOS.Core.Enums;
global using AndOS.Core.Extensions;
global using AndOS.Core.StorageConfigs;
global using AndOS.Domain.Entities;
global using AndOS.Shared.Consts;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using System.Security.Claims;
global using File = AndOS.Domain.Entities.File;
using AndOS.Infrastructure.CloudStorage;
using AndOS.Infrastructure.Custom;
using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Identity;
using AndOS.Infrastructure.Repositories;
using AndOS.Infrastructure.Validator;

namespace AndOS.Infrastructure;
public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomService();
        services.AddIdentityService(configuration);
        services.AddValidatorService();
        services.AddRepositories();
        services.AddCloudStorageService();
        services.AddDatabaseService(configuration);
        return services;
    }
}