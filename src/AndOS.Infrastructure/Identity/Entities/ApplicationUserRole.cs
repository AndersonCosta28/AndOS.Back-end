namespace AndOS.Infrastructure.Identity.Entities;

public class ApplicationUserRole : IdentityUserRole<Guid>, IUserRole
{
    public IUser User { get; set; }
    public IRole Role { get; set; }
}