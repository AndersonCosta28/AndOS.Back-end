using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetUserClaimByUserIdAndTypeAndValueSpec : Specification<IUserClaim>
{
    public GetUserClaimByUserIdAndTypeAndValueSpec(Guid userId, string claimType, string value)
    {
        Query.Where(x => x.UserId == userId)
             .Where(x => x.ClaimType == claimType)
             .Where(x => x.ClaimValue == value);
    }
}
