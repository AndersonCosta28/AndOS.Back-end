using AndOS.Application.Interfaces;
using AndOS.Application.UserPreferences.Common;
using AndOS.Shared.Requests.UserPreferences.Update;

namespace AndOS.Application.UserPreferences.Update;
public class UpdateLanguageHandler(IRepository<UserPreference> userPreferenceRepository, ICurrentUserContext currentUserContext) : IRequestHandler<UpdateLanguageRequest>
{
    public async Task Handle(UpdateLanguageRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();
        var preference = await userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(currentUserId), cancellationToken);
        if (preference == null)
        {
            preference = new();
            preference.UpdateLanguage(request.Language);
            await userPreferenceRepository.AddAsync(preference, cancellationToken);
        }
        else
        {
            preference.UpdateLanguage(request.Language);
            await userPreferenceRepository.UpdateAsync(preference, cancellationToken);
        }
    }
}
