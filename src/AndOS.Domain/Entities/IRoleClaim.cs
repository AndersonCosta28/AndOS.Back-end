using AndOS.Common.Interfaces;
using System.Security.Claims;

namespace AndOS.Domain.Entities;

public interface IRoleClaim : IAggregateRoot
{
    public Guid RoleId { get; set; }

    public string ClaimValue { get; set; }
    public string ClaimType { get; set; }

    Claim ToClaim();
}
