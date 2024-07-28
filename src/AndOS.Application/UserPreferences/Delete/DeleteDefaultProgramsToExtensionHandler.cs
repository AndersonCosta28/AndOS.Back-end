using AndOS.Application.Interfaces;
using AndOS.Application.UserPreferences.Common;
using AndOS.Shared.Requests.UserPreferences.Delete;

namespace AndOS.Application.UserPreferences.Delete;
public class DeleteDefaultProgramsToExtensionHandler(IRepository<UserPreference> userPreferenceRepository, ICurrentUserContext currentUserContext) : IRequestHandler<DeleteDefaultProgramToExtensionRequest>
{
    public async Task Handle(DeleteDefaultProgramToExtensionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        var preference = await userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(currentUserId), cancellationToken);
        if (preference == null)        
            return;        
        else
        {
            preference.RemoveDefaultProgramToExtension(request.Extension);
            await userPreferenceRepository.UpdateAsync(preference, cancellationToken);
        }
    }
}
