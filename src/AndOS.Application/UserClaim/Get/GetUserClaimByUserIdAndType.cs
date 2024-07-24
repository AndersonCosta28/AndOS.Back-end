using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetUserClaimByUserIdAndType : Specification<IUserClaim>
{
    public GetUserClaimByUserIdAndType(Guid userId, string claimType)
    {
        Query.Where(x => x.UserId == userId)
             .Where(x => x.ClaimType == claimType);
    }
}
