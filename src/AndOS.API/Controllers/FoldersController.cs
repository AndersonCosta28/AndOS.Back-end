using AndOS.Core.Enums;
using AndOS.Infrastructure.Identity.Attributes;
using AndOS.Resources.Localization;
using AndOS.Shared.Requests.Folders.Create;
using AndOS.Shared.Requests.Folders.Delete;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;
using AndOS.Shared.Requests.Folders.Update.Rename;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AndOS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<FoldersController> _logger;
    private readonly Domain.Interfaces.IAuthorizationService _authorizationService;
    private readonly IStringLocalizer<ValidationResource> _validationLocalizer;

    public FoldersController(ISender sender, ILogger<FoldersController> logger,
        Domain.Interfaces.IAuthorizationService authorizationService,
        IStringLocalizer<ValidationResource> validationLocalizer)
    {
        _sender = sender;
        _logger = logger;
        _authorizationService = authorizationService;
        _validationLocalizer = validationLocalizer;
    }

    [HttpPost]
    [AuthorizeFolderPermission(FolderPermission.Write, FolderFields.ParentFolderId)]
    public async Task<IActionResult> Create([FromBody] CreateFolderRequest request)
    {
        var response = await _sender.Send(request);
        return Created($"/api/accounts/{response.Id}", response);
    }

    [HttpGet(nameof(GetById))]
    [AuthorizeFolderPermission(FolderPermission.Read, FolderFields.Id)]
    public async Task<IActionResult> GetById([FromQuery] GetFolderByIdRequest request)
    {
        GetFolderByIdResponse result = await _sender.Send(request);
        return Ok(result);
    }

    [HttpGet(nameof(GetByPath))]
    [AuthorizeFolderPermission(FolderPermission.Read, FolderFields.Path)]
    public async Task<IActionResult> GetByPath([FromQuery] GetFolderByPathRequest request)
    {
        GetFolderByPathResponse result = await _sender.Send(request);
        return Ok(result);
    }

    [HttpPut(nameof(Rename))]
    [AuthorizeFolderPermission(FolderPermission.Rename, FolderFields.Id)]
    public async Task<IActionResult> Rename([FromBody] RenameFolderRequest request)
    {
        await _sender.Send(request);
        return NoContent();
    }

    [HttpDelete]
    [AuthorizeFolderPermission(FolderPermission.Delete, FolderFields.Id)]
    public async Task<IActionResult> Delete(DeleteFolderRequest request)
    {
        await _sender.Send(request);
        return NoContent();
    }
}