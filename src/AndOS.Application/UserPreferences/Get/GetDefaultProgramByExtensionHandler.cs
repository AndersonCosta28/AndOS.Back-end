using AndOS.Application.Interfaces;
using AndOS.Application.UserPreferences.Common;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;

namespace AndOS.Application.UserPreferences.Get;
public class GetDefaultProgramByExtensionHandler(IRepository<UserPreference> userPreferenceRepository, ICurrentUserContext currentUserContext) : IRequestHandler<GetDefaultProgramByExtensionRequest, GetDefaultProgramByExtensionResponse>
{
    public async Task<GetDefaultProgramByExtensionResponse> Handle(GetDefaultProgramByExtensionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserContext.GetCurrentUserId();

        var preference = await userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(currentUserId), cancellationToken);

        if (preference == null)
            return returnDefault(request.Extension);
        else
        {
            var defaultProgramForExtension = preference.DefaultProgramsToExtensions.Find(x => x.Extension == request.Extension);
            var result = defaultProgramForExtension is not null ? new GetDefaultProgramByExtensionResponse(new(defaultProgramForExtension.Extension, defaultProgramForExtension.Program))
                                                                                            : returnDefault(request.Extension);
            return result;
        }
    }
    GetDefaultProgramByExtensionResponse returnDefault(string extension) => new GetDefaultProgramByExtensionResponse(new(extension, string.Empty));
}
