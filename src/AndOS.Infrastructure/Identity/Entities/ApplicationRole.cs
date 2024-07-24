namespace AndOS.Infrastructure.Identity.Entities;

public class ApplicationRole : IdentityRole<Guid>, IRole
{
    public DateTime Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime Updated { get; set; }
    public Guid UpdatedBy { get; set; }
}
