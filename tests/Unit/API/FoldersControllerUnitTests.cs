using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Unit.API;

public class FoldersControllerUnitTests
{
    private readonly Mock<ISender> _mockSender;
    private readonly Mock<ILogger<FoldersController>> _mockLogger;
    private readonly Mock<IAuthorizationService> _mockAuthorizationService;
    private readonly Mock<IStringLocalizer<ValidationResource>> _mockValidationLocalizer;
    private readonly FoldersController _controller;

    public FoldersControllerUnitTests()
    {
        _mockSender = new Mock<ISender>();
        _mockLogger = new Mock<ILogger<FoldersController>>();
        _mockAuthorizationService = new Mock<IAuthorizationService>();
        _mockValidationLocalizer = new Mock<IStringLocalizer<ValidationResource>>();
        _controller = new FoldersController(_mockSender.Object, _mockLogger.Object, _mockAuthorizationService.Object, _mockValidationLocalizer.Object);
    }

    [Fact]
    public async Task Create_ReturnsNoContentResult()
    {
        // Arrange
        var response = new CreateFolderResponse(Guid.NewGuid());
        var request = new CreateFolderRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = Assert.IsType<CreatedResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var request = new GetFolderByIdRequest { Id = Guid.NewGuid() };
        var response = new GetFolderByIdResponse { Id = Guid.NewGuid(), Name = "folder-name" };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetByPath_ReturnsOkResult()
    {
        // Arrange
        var request = new GetFolderByPathRequest { Path = "folder-path" };
        var response = new GetFolderByPathResponse { Id = Guid.NewGuid(), Name = "folder-name" };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(response);

        // Act
        var result = await _controller.GetByPath(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Rename_ReturnsNoContentResult()
    {
        // Arrange
        var request = new RenameFolderRequest { /* inicialize as propriedades necessárias */ };
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
        var request = new DeleteFolderRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}