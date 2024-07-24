using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using System.Globalization;

namespace Integration.API.Factories;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public AppDbContext AppDbContext { get; private set; }
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
        builder.ConfigureServices((context, services) =>
           {
               var confiration = context.Configuration;
               RemoveServiceDescriptors(services, typeof(DbContextOptions<AppDbContext>));
               RemoveServiceDescriptors(services, typeof(AppDbContext));
               RemoveServiceDescriptors(services, typeof(DbConnection));
               RemoveServiceDescriptors(services, typeof(UserManager<ApplicationUser>));
               RemoveServiceDescriptors(services, typeof(RoleManager<ApplicationRole>));
               RemoveServiceDescriptors(services, typeof(IRoleStore<ApplicationRole>));
               RemoveServiceDescriptors(services, typeof(IEmailSender<ApplicationUser>));
               RemoveServiceDescriptors(services, typeof(IAuthorizationService));
               RemoveServiceDescriptors(services, typeof(ICurrentUserContext));
               RemoveServiceDescriptors(services, typeof(IDbContextPool<AppDbContext>));
               RemoveServiceDescriptors(services, typeof(AppDbContextFactory));

               //Reconfigurar serviços
               services.AddSingleton<DbConnection>(container =>
               {
                   var connectionString = new SqliteConnectionStringBuilder
                   {
                       DataSource = ":memory:"
                   }.ToString();
                   _connection = new SqliteConnection(connectionString);
                   _connection.Open();

                   return _connection;
               });

               services.AddPooledDbContextFactory<AppDbContext>((IServiceProvider sp, DbContextOptionsBuilder dbContextOptionsBuilder) =>
               {
                   var connection = sp.GetRequiredService<DbConnection>();
                   dbContextOptionsBuilder.UseSqlite(connection);
               });
               services.AddScoped<AppDbContextFactory>();
               services.AddScoped(sp => sp.GetRequiredService<AppDbContextFactory>().CreateDbContext());

               services.AddScoped<ICurrentUserContext, CurrentUserContext>();
               services.AddScoped<UserManager<ApplicationUser>, CustomUserManager>();
               services.AddScoped<RoleManager<ApplicationRole>>();
               services.AddScoped<IRoleStore<ApplicationRole>, RoleStore<ApplicationRole, AppDbContext, Guid>>();
               services.AddTransient<IAuthorizationService, AuthorizationService>();
           });
    }

    private void RemoveServiceDescriptors(IServiceCollection services, Type serviceType)
    {
        var descriptors = services.Where(d => d.ServiceType == serviceType).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        var scope = host.Services.CreateScope();
        AppDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return host;
    }

    public override async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        if (AppDbContext != null)
        {
            await AppDbContext.Database.EnsureDeletedAsync();
            await AppDbContext.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}