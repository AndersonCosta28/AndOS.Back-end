using Ardalis.Specification;

namespace AndOS.Application.UserPreferences.Common;
public class GetUserPreferenceByUserIdSpec : Specification<UserPreference>
{
    public GetUserPreferenceByUserIdSpec(Guid userId)
    {
        Query
            .Where(x => x.UserId == userId);
    }
}
