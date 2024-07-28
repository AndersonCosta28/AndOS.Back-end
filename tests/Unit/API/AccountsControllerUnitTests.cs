using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Unit.API;

public class AccountsControllerUnitTests
{
    private readonly Mock<ISender> _mockSender;
    private readonly Mock<ILogger<AccountsController>> _mockLogger;
    private readonly AccountsController _controller;

    public AccountsControllerUnitTests()
    {
        _mockSender = new Mock<ISender>();
        _mockLogger = new Mock<ILogger<AccountsController>>();
        _controller = new AccountsController(_mockSender.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Arrange
        _mockSender.Setup(sender => sender.Send(It.IsAny<GetAllAccontsRequest>(), default))
                   .ReturnsAsync([]);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<AccountDTO>>(okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var request = new GetAccountByIdRequest { Id = Guid.NewGuid() };
        _mockSender.Setup(sender => sender.Send(It.IsAny<GetAccountByIdRequest>(), default))
                   .ReturnsAsync(new GetAccountByIdResponse());

        // Act
        var result = await _controller.GetById(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GetAccountByIdResponse>(okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult()
    {
        // Arrange
        var response = new CreateAccountResponse(Guid.NewGuid());
        var request = new CreateAccountRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResult()
    {
        // Arrange
        var request = new UpdateAccountRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var request = new DeleteAccountRequest { /* inicialize as propriedades necessárias */ };
        _mockSender.Setup(sender => sender.Send(request, default))
                   .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetConfig_ReturnsOkResult()
    {
        // Arrange
        var request = new GetConfigByAccountIdRequest { AccountId = Guid.NewGuid() };
        var jsonDocument = JsonDocument.Parse("{\"key\":\"value\"}");
        _mockSender.Setup(sender => sender.Send(It.IsAny<GetConfigByAccountIdRequest>(), default))
                   .ReturnsAsync(jsonDocument);

        // Act
        var result = await _controller.GetConfig(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<JsonDocument>(okResult.Value);
    }
}