using AndOS.Infrastructure.Identity.Entities;
using MediatR;

namespace AndOS.Infrastructure.Identity.Actions.GenerateJwtToken;

public class GenerateJwtTokenRequest(ApplicationUser user) : IRequest<string>
{
    public ApplicationUser User { get; set; } = user;
}
