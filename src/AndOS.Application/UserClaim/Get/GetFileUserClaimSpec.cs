using AndOS.Core.Constants;
using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetFileUserClaimSpec : Specification<IUserClaim>
{
    public GetFileUserClaimSpec(Guid fileId, string value)
    {
        Query
            .Where(x => x.ClaimType.StartsWith(ClaimConsts.SCOPE_FILE))
             .Where(x => x.ClaimType.Contains(fileId.ToString()))
             .Where(x => x.ClaimValue == value);
    }
}
