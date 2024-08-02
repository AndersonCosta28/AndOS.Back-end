using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.API.Controllers;

[Collection(nameof(NoParallelizationCollection))]
public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServiceScope _scope;
    private readonly IStringLocalizer<ValidationResource> _validationResource;
    private readonly ApiUtilsTests _apiUtilsTests;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _validationResource = _scope.ServiceProvider.GetRequiredService<IStringLocalizer<ValidationResource>>();
        _apiUtilsTests = new ApiUtilsTests();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _factory.AppDbContext.Users.RemoveRange(_factory.AppDbContext.Users);
        _factory.AppDbContext.SaveChanges();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithValidCredentials()
    {
        // Arrange
        await _apiUtilsTests.CreateUserAsync(_client);
        var request = _apiUtilsTests.LoginRequest;
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
        Assert.IsType<string>(responseString);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WithInvalidCredentials()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "invalid@example.com",
            Password = "InvalidPassword",
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }


    [Fact]
    public async Task Register_ReturnsNoContent_WithValidData()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "NewPassword123!",
            UserName = "newuser"
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidPassword_ShouldFail()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "testuser@example.com",
            UserName = "TestUser",
            Password = "short"
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var result = await _client.PostAsync("/api/auth/register", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        var responseString = await result.Content.ReadFromJsonAsync<ErrorDTO>();
        Assert.Contains("Passwords must be at least 6 characters.", responseString.Detail);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateUserNameName_ShouldFail()
    {
        // Arrange
        var request1 = new RegisterRequest
        {
            Email = "user1@example.com",
            Password = "ValidPassword123!",
            UserName = "validUser"
        };

        var request2 = new RegisterRequest
        {
            Email = "user2@example.com",
            Password = "ValidPassword123!",
            UserName = "validUser"
        };
        var content1 = new StringContent(JsonSerializer.Serialize(request1), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(request2), Encoding.UTF8, "application/json");

        // Act
        var firstResponse = await _client.PostAsync("/api/auth/register", content1);
        var secondResponse = await _client.PostAsync("/api/auth/register", content2);

        // Assert
        firstResponse.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, secondResponse.StatusCode);
        var responseString = await secondResponse.Content.ReadFromJsonAsync<ErrorDTO>();
        Assert.Contains(_validationResource["UserNameAlreadyExists"].Value, responseString.Detail);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ShouldFail()
    {
        // Arrange
        var request1 = new RegisterRequest
        {
            Email = "duplicateuser@example.com",
            Password = "ValidPassword123!",
            UserName = "User1"
        };

        var request2 = new RegisterRequest
        {
            Email = "duplicateuser@example.com",
            Password = "ValidPassword123!",
            UserName = "User2"
        };
        var content1 = new StringContent(JsonSerializer.Serialize(request1), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(request2), Encoding.UTF8, "application/json");

        // Act
        var firstResponse = await _client.PostAsync("/api/auth/register", content1);
        var secondResponse = await _client.PostAsync("/api/auth/register", content2);

        // Assert
        firstResponse.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, secondResponse.StatusCode);
        var responseString = await secondResponse.Content.ReadFromJsonAsync<ErrorDTO>();
        Assert.Contains(_validationResource["EmailAlreadyExists"].Value, responseString.Detail);
    }
}