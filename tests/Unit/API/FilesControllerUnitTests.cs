using AndOS.Core.Enums;
using AndOS.Shared.Requests.Files.Create;
using AndOS.Shared.Requests.Files.Delete;
using AndOS.Shared.Requests.Files.Get.GetById;
using AndOS.Shared.Requests.Files.Update.Content;
using AndOS.Shared.Requests.Files.Update.Rename;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Unit.API;
public class FilesControllerUnitTests
{
    private readonly Mock<ISender> _mockSender;
    private readonly Mock<ILogger<FilesController>> _mockLogger;
    private readonly Mock<IAuthorizationService> _mockAuthorizationService;
    private readonly Mock<IStringLocalizer<ValidationResource>> _mockValidationLocalizer;
    private readonly FilesController _controller;

    public FilesControllerUnitTests()
    {
        _mockSender = new Mock<ISender>();
        _controller = new FilesController(_mockSender.Object);
    }

    [Fact]
    public async Task Create_ReturnsOkResult()
    {
        // Arrange
        var request = new CreateFileRequest();
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(new CreateFileResponse());

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(createdResult.Value);
        Assert.IsType<CreateFileResponse>(createdResult.Value);
    }

    [Fact]
    public async Task UpdateContent_ReturnsOkResult()
    {
        // Arrange
        var request = new UpdateContentFileRequest(Guid.NewGuid());
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(new UpdateContentFileResponse(string.Empty));

        // Act
        var result = await _controller.UpdateContent(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.IsType<UpdateContentFileResponse>(okResult.Value);
    }

    [Fact]
    public async Task Rename_ReturnsNoContentResult()
    {
        // Arrange
        var request = new RenameFileRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Rename(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var request = new DeleteFileRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var account = It.IsAny<CloudStorage>();
        var request = new GetFileByIdRequest { Id = Guid.NewGuid() };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(new GetFileByIdResponse("url", account));

        // Act
        var result = await _controller.GetById(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var response = Assert.IsType<GetFileByIdResponse>(okResult.Value);
        Assert.Equal(account, response.CloudStorage);
    }
}