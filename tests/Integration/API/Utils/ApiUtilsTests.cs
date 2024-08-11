using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Auth;
using AndOS.Shared.Requests.Auth.Login;
using AndOS.Shared.Requests.Auth.Register;

namespace Integration.API.Utils;

public class ApiUtilsTests
{
    public AzureBlobStorageConfig AzuriteConfig = new()
    {
        DefaultEndpointsProtocol = "http",
        AccountName = "devstoreaccount1",
        AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==",
        EndpointSuffix = "127.0.0.1:10000",
        ContainerName = "tests",
        IsAzurite = true
    };

    public readonly LoginRequest LoginRequest = new()
    {
        Email = "test@example.com",
        Password = "Password123!"
    };

    public readonly ApplicationUser UserTest = new()
    {
        UserName = "testuser",
        Email = "test@example.com",
        EmailConfirmed = true,
    };

    public async Task CreateUserAsync(HttpClient httpClient)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            UserName = "testuser"
        };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/api/auth/register", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task SetAuthInHttpClientAsync(HttpClient httpClient)
    {
        await CreateUserAsync(httpClient);
        var token = await GetTokenAsync(httpClient);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string> GetTokenAsync(HttpClient httpClient)
    {
        var content = new StringContent(JsonSerializer.Serialize(LoginRequest), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("/api/auth/login", content);
        response.EnsureSuccessStatusCode();
        var responseLogin = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return responseLogin.Token;
    }


    public async Task<Guid> CreateAzuriteAccontAsync(HttpClient httpClient)
    {
        var createRequest = new CreateAccountRequest
        {
            Name = "Azurite",
            CloudStorage = CloudStorage.Azure_BlobStorage,
            Config = AzuriteConfig.ToJsonDocument(),
        };

        var createResponse = await httpClient.PostAsJsonAsync("/api/accounts", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var responseJson = await createResponse.Content.ReadFromJsonAsync<CreateAccountResponse>()
                                            ?? throw new NullReferenceException();
        return responseJson.Id;
    }

    public async Task<GetAccountByIdResponse> GetAzuriteAccountAsync(HttpClient httpClient, Guid id)
    {
        var response = await httpClient.GetAsync($"/api/accounts/GetById?Id={id}");
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<GetAccountByIdResponse>()
                                             ?? throw new NullReferenceException();
        return responseJson;
    }
}