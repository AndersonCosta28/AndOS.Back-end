using AndOS.Infrastructure.Exceptions;
using AndOS.Infrastructure.Identity.Entities;
using AndOS.Shared.Requests.Auth;
using MediatR;
using System.Net;

namespace AndOS.Infrastructure.Identity.Actions.Register;

public class RegisterHandler(
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager,
    Domain.Interfaces.IAuthorizationService authorizationService) : IRequestHandler<RegisterRequest>
{
    public Task Handle(RegisterRequest request, CancellationToken cancellationToken) =>
        unitOfWork.ExecuteAsync(async (CancellationToken ct) =>
        {
            var user = new ApplicationUser(request.UserName)
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InfrastructureLayerException(message); 
            }

            List<IUserClaim> claimsToAdd = [authorizationService.CreateUserClaim(user.Folder.Id, FolderPermission.Read, ClaimConsts.VALUE_TRUE)];

            foreach (var claim in claimsToAdd)
                await authorizationService.UpdatePermissionAsync(user, claim, ct);

        }, cancellationToken);
}
