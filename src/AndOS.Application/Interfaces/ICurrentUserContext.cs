using System.Security.Claims;

namespace AndOS.Application.Interfaces;

public interface ICurrentUserContext
{
    Guid GetCurrentUserId();
    ClaimsPrincipal GetCurrentClaimPrincipal();
    string GetCurrentUserName();
    Task<IUser> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}