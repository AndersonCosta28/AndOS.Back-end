using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Delete;
using AndOS.Shared.Requests.Accounts.Get.GetAll;
using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Accounts.Get.GetConfigByAccountId;
using AndOS.Shared.Requests.Accounts.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndOS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AccountsController(ISender sender, ILogger<AccountsController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<AccountsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAllAccontsRequest()));

    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById([FromQuery] GetAccountByIdRequest request) => Ok(await _sender.Send(request));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var response = await _sender.Send(request);
        return Created($"/api/accounts/{response.Id}", response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
    {
        await _sender.Send(request);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] DeleteAccountRequest request)
    {
        await _sender.Send(request);
        return NoContent();
    }

    [HttpGet("Config")]
    public async Task<IActionResult> GetConfig([FromQuery] GetConfigByAccountIdRequest request)
    {
        return Ok(await _sender.Send(request));
    }
}