using Ardalis.Specification;

namespace AndOS.Application.RoleClaim.Get;

public class GetRoleClaimByRoleIdAndType : Specification<IRoleClaim>
{
    public GetRoleClaimByRoleIdAndType(Guid roleId, string claimType)
    {
        Query.Where(x => x.RoleId == roleId)
             .Where(x => x.ClaimType == claimType);
    }
}
