using AndOS.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.UserNameUpdated).HasDefaultValue(false);

        builder.HasMany(a => a.Accounts)
            .WithOne(a => (ApplicationUser)a.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Folder)
                .WithOne(f => (ApplicationUser)f.User)
                .HasForeignKey<Folder>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}