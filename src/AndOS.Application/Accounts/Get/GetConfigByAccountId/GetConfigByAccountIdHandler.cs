using AndOS.Application.Accounts.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Accounts.Get.GetConfigByAccountId;
using System.Text.Json;

namespace AndOS.Application.Accounts.Get.GetConfigByAccountId;

public class GetConfigByAccountIdHandler(IReadRepository<Account> accountReadRepository) : IRequestHandler<GetConfigByAccountIdRequest, JsonDocument>
{
    public async Task<JsonDocument> Handle(GetConfigByAccountIdRequest request, CancellationToken cancellationToken)
    {
        Account account = await accountReadRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(request.AccountId), cancellationToken);
        return account.Config;
    }
}