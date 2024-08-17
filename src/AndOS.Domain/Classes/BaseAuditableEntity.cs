using AndOS.Domain.Interfaces;

namespace AndOS.Domain.Classes;

public abstract class BaseAuditableEntity : BaseEntity, IAuditable
{
    public BaseAuditableEntity() : base() { }

    public DateTime Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime Updated { get; set; }
    public Guid UpdatedBy { get; set; }
}