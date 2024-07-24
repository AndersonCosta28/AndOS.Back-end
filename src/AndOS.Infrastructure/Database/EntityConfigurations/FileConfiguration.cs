namespace AndOS.Infrastructure.Database.EntityConfigurations;

using AndOS.Domain.Entities;
using AndOS.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired();

        builder.HasOne(x => (ApplicationUser)x.Owner)
            .WithMany()
            .IsRequired();

        builder.Navigation(x => (ApplicationUser)x.Owner)
            .AutoInclude();

        builder.HasIndex(f => new { f.NormalizedName, f.ParentFolderId })
            .IsUnique();
    }
}