using AndOS.Application.Interfaces;
using AndOS.Core.Constants;
using AndOS.Core.Enums;
using AndOS.Shared.Requests.Accounts.Create;

namespace AndOS.Application.Accounts.Create;

public class CreateAccountHandler(IUnitOfWork unitOfWork,
    IRepository<Account> accountRepository,
    ICurrentUserContext currentUserContext,
    Domain.Interfaces.IAuthorizationService authorizationService) : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
{
    public Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        return unitOfWork.ExecuteAsync(async (CancellationToken ct) =>
        {
            // Get user
            var currentUser = await currentUserContext.GetCurrentUserAsync(ct);

            // Create account
            var account = new Account(request.Name, request.CloudStorage, currentUser, request.Config);

            await accountRepository.AddAsync(account, ct);

            List<IUserClaim> claimsToAdd =
            [
                authorizationService.CreateUserClaim(account.Folder.Id, FolderPermission.Read, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(account.Folder.Id, FolderPermission.Shared, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(account.Folder.Id, FolderPermission.Write, ClaimConsts.VALUE_TRUE),
            ];

            foreach (var claim in claimsToAdd)
                await authorizationService.UpdatePermissionAsync(currentUser, claim, ct);

            return new CreateAccountResponse(account.Id);

        }, cancellationToken);
    }
}