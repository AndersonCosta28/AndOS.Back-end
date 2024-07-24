using AndOS.Application.Accounts.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Accounts.Update;

namespace AndOS.Application.Accounts.Update;

public class UpdateAccountHandler(IRepository<Account> accountRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAccountRequest>
{
    public Task Handle(UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        return unitOfWork.ExecuteAsync(async (ct) =>
        {
            var account = await accountRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(request.Id), ct);
            account.UpdateCloudStorage(request.CloudStorage);
            account.UpdateName(request.Name);
            account.UpdateConfig(request.Config);
            if (account.Folder.Name != request.Name)
                account.Folder.UpdateName(request.Name);
            await accountRepository.UpdateAsync(account, ct);
        }, cancellationToken);
    }
}