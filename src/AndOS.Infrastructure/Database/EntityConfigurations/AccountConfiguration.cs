using AndOS.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired();

        builder.Property(a => a.Config)
            .HasConversion(new JsonDocumentConverter());

        builder.HasOne(a => a.Folder)
            .WithOne(f => f.Account)
            .HasForeignKey<Account>(a => a.FolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}