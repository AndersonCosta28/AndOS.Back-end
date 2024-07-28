using Bogus;
using Integration.API.Factories;
using Integration.API.Utils;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Integration.API.Controllers;

[Collection(nameof(NoParallelizationCollection))]
public class FilesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApiUtilsTests _apiUtilsTests;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private Guid _accountId;
    private GetAccountByIdResponse _account;

    public FilesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _apiUtilsTests = new();
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
    public async Task Create_ReturnsOkResult()
    {
        // Arrange
        var request = new CreateFileRequest(_faker.Random.String2(1, 100), _faker.Random.String2(1, 3), "10mb", _account.Folder.Id);
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Files", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadFromJsonAsync<CreateFileResponse>();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseJson);
        Assert.IsType<Guid>(responseJson.Id);
        Assert.Equal(_account.CloudStorage, responseJson.CloudStorage);
        Assert.StartsWith($"{_apiUtilsTests.AzuriteConfig.Url}/{responseJson.Id}", responseJson.Url);
    }

    [Fact]
    public async Task UpdateContent_ReturnsOkResult()
    {
        // Arrange
        var requestCreated = new CreateFileRequest(_faker.Random.String2(1, 100), _faker.Random.String2(1, 3), "10mb", _account.Folder.Id);
        var contentCreate = new StringContent(JsonSerializer.Serialize(requestCreated), Encoding.UTF8, "application/json");
        var responseCreated = await _client.PostAsync("/api/Files", contentCreate);
        responseCreated.EnsureSuccessStatusCode();
        var responseCreatedJson = await responseCreated.Content.ReadFromJsonAsync<CreateFileResponse>();

        var request = new UpdateContentFileRequest(responseCreatedJson.Id);
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/Files/UpdateContent", content);

        // Assert
        var responseJson = await response.Content.ReadFromJsonAsync<UpdateContentFileResponse>();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseJson);
        Assert.IsType<string>(responseJson.Url);
        Assert.StartsWith($"{_apiUtilsTests.AzuriteConfig.Url}/{responseCreatedJson.Id}", responseJson.Url);
    }

    [Fact]
    public async Task Rename_ReturnsNoContentResult()
    {
        // Arrange
        var requestCreated = new CreateFileRequest(_faker.Random.String2(1, 100), _faker.Random.String2(1, 3), "10mb", _account.Folder.Id);
        var contentCreate = new StringContent(JsonSerializer.Serialize(requestCreated), Encoding.UTF8, "application/json");
        var responseCreated = await _client.PostAsync("/api/Files", contentCreate);
        responseCreated.EnsureSuccessStatusCode();
        var responseCreatedJson = await responseCreated.Content.ReadFromJsonAsync<CreateFileResponse>();

        var request = new RenameFileRequest
        {
            Id = responseCreatedJson.Id,
            Name = _faker.Random.String2(1, 100),
            Extension = _faker.Random.String2(1, 3),
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/Files/Rename", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult()
    {
        // Arrange
        var requestCreated = new CreateFileRequest(_faker.Random.String2(1, 100), _faker.Random.String2(1, 3), "10mb", _account.Folder.Id);
        var contentCreate = new StringContent(JsonSerializer.Serialize(requestCreated), Encoding.UTF8, "application/json");
        var responseCreated = await _client.PostAsync("/api/Files", contentCreate);
        responseCreated.EnsureSuccessStatusCode();
        var responseCreatedJson = await responseCreated.Content.ReadFromJsonAsync<CreateFileResponse>();
        await UploadAsync(responseCreatedJson.Url, "teste");

        // Act
        var response = await _client.DeleteAsync($"/api/Files?Id={responseCreatedJson.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var requestCreated = new CreateFileRequest(_faker.Random.String2(1, 100), _faker.Random.String2(1, 3), "10mb", _account.Folder.Id);
        var contentCreate = new StringContent(JsonSerializer.Serialize(requestCreated), Encoding.UTF8, "application/json");
        var responseCreated = await _client.PostAsync("/api/Files", contentCreate);
        responseCreated.EnsureSuccessStatusCode();
        var responseCreatedJson = await responseCreated.Content.ReadFromJsonAsync<CreateFileResponse>();
        await UploadAsync(responseCreatedJson.Url, "teste");

        // Act
        var response = await _client.GetAsync($"/api/Files/GetById?Id={responseCreatedJson.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<GetFileByIdResponse>();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseJson);
        Assert.IsType<GetFileByIdResponse>(responseJson);
        Assert.StartsWith($"{_apiUtilsTests.AzuriteConfig.Url}/{responseCreatedJson.Id}", responseJson.Url);
        Assert.Equal(_account.CloudStorage, responseJson.CloudStorage);
        var content = await DownloadAsync(responseJson.Url);
        Assert.Equal("teste", content);
    }

    async Task UploadAsync(string url, string data, CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new();
        HttpContent content = new StringContent(data, Encoding.UTF8, "text/plain");
        content.Headers.ContentLength = data.Length;
        content.Headers.Add("x-ms-blob-type", "BlockBlob");
        content.Headers.Add("x-ms-version", AzureConsts.DefaultServiceVersion);

        HttpResponseMessage response = await httpClient.PutAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    async Task<string> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        return content;
    }
}