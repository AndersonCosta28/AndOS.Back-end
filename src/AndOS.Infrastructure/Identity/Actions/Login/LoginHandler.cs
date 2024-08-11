using AndOS.Infrastructure.Identity.Actions.GenerateJwtToken;
using AndOS.Infrastructure.Identity.Entities;
using AndOS.Shared.Requests.Auth;
using AndOS.Shared.Requests.Auth.Login;
using MediatR;

namespace AndOS.Infrastructure.Identity.Actions.Login;

public class LoginHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ISender sender) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new HttpRequestException(null, null, System.Net.HttpStatusCode.Unauthorized);
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!result.Succeeded)
            throw new HttpRequestException(null, null, System.Net.HttpStatusCode.Unauthorized);

        var token = await sender.Send(new GenerateJwtTokenRequest(user), cancellationToken);

        return new(token);
    }
}
