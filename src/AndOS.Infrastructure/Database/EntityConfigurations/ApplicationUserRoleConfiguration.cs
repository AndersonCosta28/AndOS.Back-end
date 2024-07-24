using AndOS.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AndOS.Infrastructure.Database.EntityConfigurations
{
    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            // Configuração da chave primária composta
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Configuração da relação com ApplicationUser
            builder.HasOne(ur => (ApplicationUser)ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // Configuração da relação com ApplicationRole
            builder.HasOne(ur => (ApplicationRole)ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Configuração das navegações
            builder.Navigation(ur => ur.User)
                .AutoInclude();

            builder.Navigation(ur => ur.Role)
                .AutoInclude();
        }
    }
}
