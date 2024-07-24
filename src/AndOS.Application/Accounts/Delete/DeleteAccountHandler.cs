using AndOS.Application.Accounts.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Accounts.Delete;

namespace AndOS.Application.Accounts.Delete;

public class DeleteAccountHandler(IRepository<Account> accountRepository) : IRequestHandler<DeleteAccountRequest>
{
    public async Task Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(request.Id), cancellationToken);
        await accountRepository.DeleteAsync(account, cancellationToken);
    }
}