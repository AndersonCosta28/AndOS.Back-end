using AndOS.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations;
public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired();

        builder.HasMany(f => f.Files)
            .WithOne(x => x.ParentFolder)
            .HasForeignKey(x => x.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Folders)
            .WithOne(x => x.ParentFolder)
            .HasForeignKey(x => x.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => (ApplicationUser)f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => (ApplicationUser)f.User)
            .WithOne(u => u.Folder)
            .HasForeignKey<Folder>(f => f.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Account)
            .WithOne(u => u.Folder)
            .HasForeignKey<Folder>(f => f.AccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.NormalizedName, f.ParentFolderId })
            .IsUnique();
    }
}