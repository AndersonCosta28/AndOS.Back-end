using AndOS.Application.Interfaces;
using AndOS.Application.UserPreferences.Common;
using AndOS.Shared.Requests.UserPreferences.Update;

namespace AndOS.Application.UserPreferences.Update;
public class UpdateDefaultProgramsToExtensionHandler(IRepository<UserPreference> userPreferenceRepository, ICurrentUserContext currentUserContext, IMapperService mapperService) : IRequestHandler<UpdateDefaultProgramsToExtensionRequest>
{
    public async Task Handle(UpdateDefaultProgramsToExtensionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        var defaultProgramToExtension = await mapperService.MapAsync<List<DefaultProgramForExtension>>(request.DefaultProgramsToExtensions);
        var preference = await userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(currentUserId), cancellationToken);
        if (preference == null)
        {
            preference = new();
            preference.UpdateDefaultProgramToExtension(defaultProgramToExtension);
            await userPreferenceRepository.AddAsync(preference, cancellationToken);
        }
        else
        {
            preference.UpdateDefaultProgramToExtension(defaultProgramToExtension);
            await userPreferenceRepository.UpdateAsync(preference, cancellationToken);
        }
    }
}