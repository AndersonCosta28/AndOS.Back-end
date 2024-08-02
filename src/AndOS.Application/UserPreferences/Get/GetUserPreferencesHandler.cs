using AndOS.Application.Interfaces;
using AndOS.Application.UserPreferences.Common;
using AndOS.Shared.Requests.UserPreferences.Get;

namespace AndOS.Application.UserPreferences.Get;
public class GetUserPreferencesHandler(IRepository<UserPreference> userPreferenceRepository, ICurrentUserContext currentUserContext, IMapperService mapperService) : IRequestHandler<GetUserPreferencesRequest, UserPreferenceDTO>
{
    public async Task<UserPreferenceDTO> Handle(GetUserPreferencesRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        var preference = await userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(currentUserId), cancellationToken);
        if (preference == null)
            return new UserPreferenceDTO("en-US", []);

        return await mapperService.MapAsync<UserPreferenceDTO>(preference);
    }
}
