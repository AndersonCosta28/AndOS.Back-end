using AndOS.Core.Constants;
using Ardalis.Specification;

namespace AndOS.Application.UserClaim.Get;

public class GetFolderUserClaimByUserIdAndFolderIdSpec : Specification<IUserClaim>
{
    public GetFolderUserClaimByUserIdAndFolderIdSpec(Guid userId, Guid folderId)
    {
        Query
            .Where(x => x.UserId.Equals(userId))
            .Where(x => x.ClaimType.StartsWith(ClaimConsts.SCOPE_FOLDER))
            .Where(x => x.ClaimType.Contains(folderId.ToString()))
            .Where(x => x.ClaimValue == ClaimConsts.VALUE_TRUE);
    }
}
