using AndOS.Application.Users.Get.GetById;
using AndOS.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AndOS.Infrastructure.Identity;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor, IServiceProvider services, ILogger<CurrentUserContext> logger) : ICurrentUserContext
{
    public async Task<IUser> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var userRepository = services.GetRequiredService<IReadRepository<IUser>>();
        var claim = httpContextAccessor.HttpContext.User;

        // Verifique se o claim está preenchido corretamente
        if (claim == null || !claim.Identity.IsAuthenticated)
        {
            logger.LogInformation("Claim is null or user is not authenticated.");
            return null;
        }

        IUser user = await userManager.GetUserAsync(claim);
        if (user == null)
        {
            logger.LogInformation("User is null.");
        }

        user = await userRepository.FirstOrDefaultAsync(new GetUserByIdSpec(user.Id), cancellationToken);

        return user;
    }

    public ClaimsPrincipal GetCurrentClaimPrincipal()
    {
        var claim = httpContextAccessor.HttpContext.User;
        return claim;
    }

    public Guid GetCurrentUserId()
    {
        return Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    public string GetCurrentUserName()
    {
        return httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    }
}
