using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Accounts.Get.GetById;

namespace AndOS.Application.Accounts.Get.GetById;

public class GetAccountByIdHandler(IReadRepository<Account> accountReadRepository) : IRequestHandler<GetAccountByIdRequest, GetAccountByIdResponse>
{
    public async Task<GetAccountByIdResponse> Handle(GetAccountByIdRequest request, CancellationToken cancellationToken)
    {
        GetAccountByIdResponse account = await accountReadRepository.ProjectToFirstOrDefaultAsync<GetAccountByIdResponse>(new GetAccountByIdSpec(request.Id), cancellationToken);
        return account;
    }
}