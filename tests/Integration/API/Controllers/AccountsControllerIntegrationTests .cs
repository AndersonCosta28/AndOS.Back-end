using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Accounts.Update;

namespace Integration.API.Controllers;

[Collection(nameof(NoParallelizationCollection))]
public class AccountsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApiUtilsTests _apiUtilsTests;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _apiUtilsTests = new ApiUtilsTests();
    }

    public async Task InitializeAsync()
    {
        await _apiUtilsTests.SetAuthInHttpClientAsync(_client);
    }

    public Task DisposeAsync()
    {
        _factory.AppDbContext.Users.RemoveRange(_factory.AppDbContext.Users);
        _factory.AppDbContext.SaveChanges();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/accounts");

        // Assert
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountDTO>>();
        Assert.IsType<List<AccountDTO>>(accounts);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var createRequest = new CreateAccountRequest
        {
            Name = "Azure",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = JsonDocument.Parse("{\"key\":\"value\"}")
        };

        var createResponse = await _client.PostAsJsonAsync("/api/accounts", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var accountCreated = await createResponse.Content.ReadFromJsonAsync<CreateAccountResponse>();

        // Act
        var response = await _client.GetAsync($"/api/accounts/GetById?Id={accountCreated.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var account = await response.Content.ReadFromJsonAsync<GetAccountByIdResponse>();
        Assert.IsType<GetAccountByIdResponse>(account);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            Name = "Azure",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = JsonDocument.Parse("{\"key\":\"value\"}")
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", request);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadFromJsonAsync<CreateAccountResponse>();
        Assert.NotNull(responseJson);
        Assert.IsType<Guid>(responseJson.Id);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResult()
    {
        // Arrange
        var createRequest = new CreateAccountRequest
        {
            Name = "Azure",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = JsonDocument.Parse("{\"key\":\"value\"}")
        };

        var createResponse = await _client.PostAsJsonAsync("/api/accounts", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var accountCreated = await createResponse.Content.ReadFromJsonAsync<CreateAccountResponse>();

        var request = new UpdateAccountRequest
        {
            Id = accountCreated.Id, // Use a valid account ID
            Name = "Aws",
            CloudStorage = CloudStorage.AWS_S3Storage,
            Config = JsonDocument.Parse("{\"key\":\"updatedvalue\"}")
        };
        // Act
        var response = await _client.PutAsJsonAsync("/api/accounts", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var createRequest = new CreateAccountRequest
        {
            Name = "Azure",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = JsonDocument.Parse("{\"key\":\"value\"}")
        };

        var createResponse = await _client.PostAsJsonAsync("/api/accounts", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var accountCreated = await createResponse.Content.ReadFromJsonAsync<CreateAccountResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/accounts?Id={accountCreated.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetConfig_ReturnsOkResult()
    {
        // Arrange
        var createRequest = new CreateAccountRequest
        {
            Name = "Azure",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = JsonDocument.Parse("{\"key\":\"value\"}")
        };

        var createResponse = await _client.PostAsJsonAsync("/api/accounts", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var accountCreated = await createResponse.Content.ReadFromJsonAsync<CreateAccountResponse>();

        // Act
        var response = await _client.GetAsync($"/api/accounts/Config?AccountId={accountCreated.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.IsType<JsonDocument>(config);
    }
}