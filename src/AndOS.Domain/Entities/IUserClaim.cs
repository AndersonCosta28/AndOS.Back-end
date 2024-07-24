using AndOS.Common.Interfaces;
using System.Security.Claims;

namespace AndOS.Domain.Entities;

public interface IUserClaim : IAggregateRoot
{
    Guid UserId { get; set; }
    string ClaimType { get; set; }
    string ClaimValue { get; set; }

    Claim ToClaim();
}
