using AndOS.Infrastructure.Identity.Attributes;
using AndOS.Shared.Requests.Files.Create;
using AndOS.Shared.Requests.Files.Delete;
using AndOS.Shared.Requests.Files.Get.GetById;
using AndOS.Shared.Requests.Files.Update.Content;
using AndOS.Shared.Requests.Files.Update.Rename;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndOS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FilesController(ISender send) : ControllerBase
{
    [HttpPost]
    [AuthorizeFolderPermission(FolderPermission.Write, FolderFields.ParentFolderId)]
    public async Task<IActionResult> Create([FromBody] CreateFileRequest request)
    {
        var response = await send.Send(request);
        return Created($"/api/files/{response.Id}", response);
    }

    [HttpPut(nameof(UpdateContent))]
    [AuthorizeFilePermission(FilePermission.Write, FileFields.Id)]
    public async Task<IActionResult> UpdateContent([FromBody] UpdateContentFileRequest request)
    {
        return Ok(await send.Send(request));
    }

    [HttpPut(nameof(Rename))]
    [AuthorizeFilePermission(FilePermission.Rename, FileFields.Id)]
    public async Task<IActionResult> Rename([FromBody] RenameFileRequest request)
    {
        await send.Send(request);
        return NoContent();
    }

    [HttpDelete]
    [AuthorizeFilePermission(FilePermission.Delete, FileFields.Id)]
    public async Task<IActionResult> Delete([FromQuery] DeleteFileRequest request)
    {
        await send.Send(request);
        return NoContent();
    }

    [HttpGet(nameof(GetById))]
    [AuthorizeFilePermission(FilePermission.Read, FileFields.Id)]
    public async Task<IActionResult> GetById([FromQuery] GetFileByIdRequest request)
    {
        return Ok(await send.Send(request));
    }
}