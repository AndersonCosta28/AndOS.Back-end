using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Accounts.Get.GetAll;

namespace AndOS.Application.Accounts.Get.GetAll;

public class GetAllAccountsHandler(ICurrentUserContext currentUserContext, IReadRepository<Account> accountReadRepository) : IRequestHandler<GetAllAccontsRequest, List<AccountDTO>>
{
    public async Task<List<AccountDTO>> Handle(GetAllAccontsRequest request, CancellationToken cancellationToken)
    {
        Guid userId = currentUserContext.GetCurrentUserId();
        List<AccountDTO> accounts = await accountReadRepository.ProjectToListAsync<AccountDTO>(new GetAllAccountsSpec(userId), cancellationToken);
        return accounts;
    }
}