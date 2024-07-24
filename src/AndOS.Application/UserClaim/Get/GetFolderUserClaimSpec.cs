using AndOS.Core.Constants;
using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetFolderUserClaimSpec : Specification<IUserClaim>
{
    public GetFolderUserClaimSpec(Guid folderId, string value)
    {
        Query
            .Where(claim => claim.ClaimType.StartsWith(ClaimConsts.SCOPE_FOLDER))
             .Where(x => x.ClaimType.Contains(folderId.ToString()))
             .Where(x => x.ClaimValue == value);
    }
}
