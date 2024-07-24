using AndOS.Core.Constants;
using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetFileUserClaimByUserIdAndFileIdSpec : Specification<IUserClaim>
{
    public GetFileUserClaimByUserIdAndFileIdSpec(Guid userId, Guid fileId)
    {
        Query
            .Where(x => x.UserId.Equals(userId))
            .Where(x => x.ClaimType.StartsWith(ClaimConsts.SCOPE_FILE))
            .Where(x => x.ClaimType.Contains(fileId.ToString()))
            .Where(x => x.ClaimValue == ClaimConsts.VALUE_TRUE);
    }
}
