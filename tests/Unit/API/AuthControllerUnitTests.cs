using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Unit.API;

public class AuthControllerUnitTests
{
    private readonly Mock<ISender> _mockSender;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerUnitTests()
    {
        _mockSender = new Mock<ISender>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(null, null, null, _mockSender.Object, null);
    }

    [Fact]
    public async Task Login_ReturnsOkResult()
    {
        // Arrange
        var request = new LoginRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync("fake-jwt-token");

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Register_ReturnsNoContentResult()
    {
        // Arrange
        var request = new RegisterRequest
        {

        };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}