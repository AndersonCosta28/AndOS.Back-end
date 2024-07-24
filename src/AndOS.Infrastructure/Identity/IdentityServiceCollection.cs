using AndOS.Domain.Interfaces;
using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Email;
using AndOS.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AndOS.Infrastructure.Identity;

public static class IdentityServiceCollection
{
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<UserManager<ApplicationUser>, CustomUserManager>();
        services.AddScoped<RoleManager<ApplicationRole>>();
        services.AddScoped<IRoleStore<ApplicationRole>, RoleStore<ApplicationRole, AppDbContext, Guid>>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();

        // Configura Identity com Entity Framework e token providers
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();

        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        // Configura a autenticação JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        // Adiciona serviços de autorização
        services.AddAuthorization();

        return services;
    }
}
