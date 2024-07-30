using AndOS.Shared.Requests.UserPreferences.Delete;
using AndOS.Shared.Requests.UserPreferences.Get;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;
using AndOS.Shared.Requests.UserPreferences.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndOS.API.Controllers;
[Authorize]
[Route("api/[controller]")]
public class UserPreferencesController(ISender sender) : ControllerBase
{
    [HttpPut(nameof(UpdateDefaultProgramsToExtension))]
    public async Task<IActionResult> UpdateDefaultProgramsToExtension([FromBody] UpdateDefaultProgramsToExtensionRequest request)
    {
        await sender.Send(request);
        return Ok();
    }

    [HttpPut(nameof(UpdateLanguage))]
    public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageRequest request)
    {
        await sender.Send(request);
        return Ok();
    }

    [HttpGet(nameof(GetUserPreferences))]
    public async Task<IActionResult> GetUserPreferences()
    {
        var result = await sender.Send(new GetUserPreferencesRequest());
        return Ok(result);
    }

    [HttpGet(nameof(GetDefaultProgramByExtension))]
    public async Task<IActionResult> GetDefaultProgramByExtension([FromQuery] GetDefaultProgramByExtensionRequest request)
    {
        var result = await sender.Send(request);
        return Ok(result);
    }

    [HttpDelete(nameof(DeleteDefaultProgramsToExtension))]
    public async Task<IActionResult> DeleteDefaultProgramsToExtension([FromQuery] DeleteDefaultProgramToExtensionRequest request)
    {
        await sender.Send(request);
        return Ok();
    }
}
