using AndOS.Domain.Classes;
using AndOS.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace AndOS.Infrastructure.Database;

public class AppDbContext :
    IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<Guid>, ApplicationRoleClaim, IdentityUserToken<Guid>>
{
    private ICurrentUserContext _currentUserContext;
    public AppDbContext()
    {
        Database.Migrate();
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    public void SetCurrentUser(ICurrentUserContext currentUserContext)
    {
        this._currentUserContext = currentUserContext;
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<File> Files => Set<File>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        beforeSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        beforeSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void beforeSave()
    {
        List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseAuditableEntity>> entities = ChangeTracker.Entries<BaseAuditableEntity>().ToList();
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseAuditableEntity> entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                if (entity.Entity.GetType() == typeof(Folder))
                {
                    Folder folder = (Folder)entity.Entity;
                    if (folder.Type == FolderType.User)
                    {
                        folder.CreatedBy = folder.User.Id;
                        folder.Created = DateTime.Now.ToUniversalTime();
                        continue;
                    }

                    entity.Property("Created").CurrentValue = DateTime.Now.ToUniversalTime();
                    entity.Property("CreatedBy").CurrentValue = _currentUserContext.GetCurrentUserId();
                }
            }
            else if (entity.State == EntityState.Modified)
            {
                entity.Property("Updated").CurrentValue = DateTime.Now.ToUniversalTime();
                entity.Property("UpdatedBy").CurrentValue = _currentUserContext.GetCurrentUserId();
            }
        }
    }
}