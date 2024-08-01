using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations;
public class DefaultProgramForExtensionConfiguration : IEntityTypeConfiguration<DefaultProgramForExtension>
{
    public void Configure(EntityTypeBuilder<DefaultProgramForExtension> builder)
    {
        builder.HasKey(x => new { x.Extension, x.Program, x.UserPreferenceId });
        builder.Property(x => x.Program).IsRequired();
        builder.Property(x => x.Extension).IsRequired();
    }
}
