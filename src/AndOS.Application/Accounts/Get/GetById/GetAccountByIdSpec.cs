using Ardalis.Specification;

namespace AndOS.Application.Accounts.Get.GetById;

public class GetAccountByIdSpec : Specification<Account>
{
    public GetAccountByIdSpec(Guid accountId)
    {
        Query
            .Include(x => x.Folder)
            .Where(x => x.Id == accountId);
    }
}