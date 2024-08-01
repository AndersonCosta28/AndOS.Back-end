using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Folders.Create;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;
using AndOS.Shared.Requests.Folders.Update.Rename;

namespace Integration.API.Controllers;

[Collection(nameof(NoParallelizationCollection))]
public class FoldersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApiUtilsTests _apiUtilsTests;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private Guid _accountId;
    private GetAccountByIdResponse _account;

    public FoldersControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _apiUtilsTests = new ApiUtilsTests();
        _faker = new();
    }

    public async Task InitializeAsync()
    {
        await _apiUtilsTests.SetAuthInHttpClientAsync(_client);
        _accountId = await _apiUtilsTests.CreateAzuriteAccontAsync(_client);
        _account = await _apiUtilsTests.GetAzuriteAccountAsync(_client, _accountId);
    }

    public Task DisposeAsync()
    {
        _factory.AppDbContext.Users.RemoveRange(_factory.AppDbContext.Users);
        _factory.AppDbContext.SaveChanges();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_ReturnsNoContentResult()
    {
        // Arrange
        var request = new CreateFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            ParentFolderId = _account.Folder.Id
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Folders", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadFromJsonAsync<CreateFolderResponse>();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseJson);
        Assert.IsType<Guid>(responseJson.Id);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var createRequest = new CreateFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            ParentFolderId = _account.Folder.Id
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/Folders", createContent);
        createResponse.EnsureSuccessStatusCode();
        var folderCreated = await createResponse.Content.ReadFromJsonAsync<CreateFolderResponse>();

        // Act
        var response = await _client.GetAsync($"/api/Folders/GetById?Id={folderCreated.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var folderResponseJson = await response.Content.ReadFromJsonAsync<GetFolderByIdResponse>();
        Assert.IsType<GetFolderByIdResponse>(folderResponseJson);
    }

    [Fact]
    public async Task GetByPath_ReturnsOkResult()
    {
        // Arrange
        var createRequest = new CreateFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            ParentFolderId = _account.Folder.Id
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/Folders", createContent);
        createResponse.EnsureSuccessStatusCode();
        var folderCreated = await createResponse.Content.ReadFromJsonAsync<CreateFolderResponse>();
        var folderGetByIdResponse = await _client.GetAsync($"/api/Folders/GetById?Id={folderCreated.Id}");
        folderGetByIdResponse.EnsureSuccessStatusCode();
        var folderGetByIdResponseJson = await folderGetByIdResponse.Content.ReadFromJsonAsync<GetFolderByIdResponse>();

        // Act
        var response = await _client.GetAsync($"/api/Folders/GetByPath?Id={folderGetByIdResponseJson.FullPath}");

        // Assert
        response.EnsureSuccessStatusCode();
        var folderResponseJson = await response.Content.ReadFromJsonAsync<GetFolderByPathResponse>();
        Assert.IsType<GetFolderByPathResponse>(folderResponseJson);
    }

    [Fact]
    public async Task Rename_ReturnsNoContentResult()
    {
        // Arrange
        var createRequest = new CreateFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            ParentFolderId = _account.Folder.Id
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/Folders", createContent);
        createResponse.EnsureSuccessStatusCode();
        var folderCreated = await createResponse.Content.ReadFromJsonAsync<CreateFolderResponse>();

        var request = new RenameFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            Id = folderCreated.Id
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/Folders/Rename?", content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var createRequest = new CreateFolderRequest
        {
            Name = _faker.Random.String2(1, 100),
            ParentFolderId = _account.Folder.Id
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/Folders", createContent);
        createResponse.EnsureSuccessStatusCode();
        var folderCreated = await createResponse.Content.ReadFromJsonAsync<CreateFolderResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/Folders?Id={folderCreated.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}