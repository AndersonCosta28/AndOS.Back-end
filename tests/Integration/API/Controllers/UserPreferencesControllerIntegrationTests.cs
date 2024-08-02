using System.Net;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.UserPreferences.Delete;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;
using AndOS.Shared.Requests.UserPreferences.Update;
using Integration.API.Utils;

namespace Integration.API.Controllers
{
    public class UserPreferencesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly ApiUtilsTests _apiUtilsTests;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UserPreferencesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            this._factory = factory;
            _client = _factory.CreateClient();
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
        public async Task UpdateDefaultProgramsToExtension_ReturnsOkResult()
        {
            // Arrange
            var request = new UpdateDefaultProgramsToExtensionRequest([new("txt", "notepad")]);

            // Act
            var response = await _client.PutAsJsonAsync("/api/UserPreferences/UpdateDefaultProgramsToExtension", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateLanguage_ReturnsOkResult()
        {
            // Arrange
            var request = new UpdateLanguageRequest("fr-FR");

            // Act
            var response = await _client.PutAsJsonAsync("/api/UserPreferences/UpdateLanguage", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUserPreferences_ReturnsOkResult()
        {
            // Act
            var response = await _client.GetAsync("/api/UserPreferences/GetUserPreferences");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userPreferences = await response.Content.ReadFromJsonAsync<UserPreferenceDTO>();
            Assert.NotNull(userPreferences);
        }

        [Fact]
        public async Task GetDefaultProgramByExtension_ReturnsOkResult()
        {
            // Arrange
            var request = new GetDefaultProgramByExtensionRequest("txt");

            // Act
            var response = await _client.GetAsync($"/api/UserPreferences/GetDefaultProgramByExtension?Extension={request.Extension}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var defaultProgram = await response.Content.ReadFromJsonAsync<DefaultProgramToExtensionDTO>();
            Assert.NotNull(defaultProgram);
        }

        [Fact]
        public async Task DeleteDefaultProgramsToExtension_ReturnsOkResult()
        {
            // Arrange
            var request = new DeleteDefaultProgramToExtensionRequest("txt");

            // Act
            var response = await _client.DeleteAsync($"/api/UserPreferences/DeleteDefaultProgramsToExtension?Extension={request.Extension}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
