using AndOS.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations;
public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => (ApplicationUser)x.User)
            .WithOne();

        builder.HasMany(x => x.DefaultProgramsToExtensions)
            .WithOne(x => x.UserPreference)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.DefaultProgramsToExtensions)
            .AutoInclude();
    }
}
