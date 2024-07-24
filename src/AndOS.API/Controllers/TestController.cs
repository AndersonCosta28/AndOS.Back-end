using AndOS.Application.Interfaces;
using AndOS.Domain.Entities;
using AndOS.Resources.Localization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
[Route("api/[controller]")]

public class TestController : ControllerBase
{
    private readonly IStringLocalizer<ValidationResource> _localizer;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IRepository<IUser> _userRepository;
    public TestController(IStringLocalizer<ValidationResource> localizer, IAuthenticationSchemeProvider schemeProvider, IRepository<IUser> userRepository)
    {
        _localizer = localizer;
        _schemeProvider = schemeProvider;
        _userRepository = userRepository;
    }

    [HttpGet("test-localization")]
    public IActionResult TestLocalization()
    {
        LocalizedString message = _localizer["IdEmpty"];
        return Ok(new { Message = message });
    }

    [HttpGet("GetAllSchemasIdentity")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllSchemasIdentity()
    {
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        var schemeNames = new List<string>();

        foreach (var scheme in schemes)
        {
            schemeNames.Add(scheme.Name);
        }

        return Ok(schemeNames);
    }
}
