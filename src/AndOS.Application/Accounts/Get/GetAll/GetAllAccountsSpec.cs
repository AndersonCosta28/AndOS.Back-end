using Ardalis.Specification;

namespace AndOS.Application.Accounts.Get.GetAll;

public class GetAllAccountsSpec : Specification<Account>
{
    public GetAllAccountsSpec(Guid userId)
    {
        Query
            .AsNoTracking()
            .Where(x => x.User.Id == userId);
    }
}