using System.Net;
using System.Net.Http.Json;

namespace StudyTeknik.Test.DeckTests
{
    // DTO för att matcha din CreateDeckCommand
    public record CreateDeckDto(string Title, string CourseName, string SubjectName);
    public record DeckResponseDto(Guid Id, string Title, string CourseName, string SubjectName);

    [TestFixture]
    public class DeckIntegrationTests
    {
        private CustomWebApplicationFactory.CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory.CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task CreateDeck_ShouldReturnCreated_WhenDataIsValid()
        {
            // 1. Arrange - Matcha CreateDeckCommand properties
            var newDeck = new CreateDeckDto(
                Title: "Integration Test Deck",
                CourseName: "Test Course", 
                SubjectName: "Test Subject"
            );

            // 2. Act
            var response = await _client.PostAsJsonAsync("/api/decks/CreateDeck", newDeck);

            // 3. Assert med bättre felmeddelanden
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Assert.Fail($"Expected success but got {response.StatusCode}. Response: {errorContent}");
            }
            
            var createdDeck = await response.Content.ReadFromJsonAsync<DeckResponseDto>();
            Assert.That(createdDeck, Is.Not.Null);
            Assert.That(createdDeck.Title, Is.EqualTo("Integration Test Deck"));
            Assert.That(createdDeck.CourseName, Is.EqualTo("Test Course"));
        }

        [Test]
        public async Task GetAllDecks_ShouldReturnList_WhenDecksExist()
        {
            // 1. Arrange - Skapa en kortlek först
            var newDeck = new CreateDeckDto(
                Title: "List Test Deck",
                CourseName: "List Course",
                SubjectName: "List Subject"
            );
            
            var createResponse = await _client.PostAsJsonAsync("/api/decks/CreateDeck", newDeck);
            
            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                Assert.Fail($"Setup failed - couldn't create deck. Status: {createResponse.StatusCode}, Response: {errorContent}");
            }

            // 2. Act - Hämta listan
            var response = await _client.GetAsync("/api/decks/GetAllDecks");

            // 3. Assert
            response.EnsureSuccessStatusCode();
            var decks = await response.Content.ReadFromJsonAsync<List<DeckResponseDto>>();
            
            Assert.That(decks, Is.Not.Null);
            Assert.That(decks.Count, Is.GreaterThan(0), "Expected at least one deck in the list");
        }

        [Test]
        public async Task GetDeck_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // 1. Arrange
            var fakeId = Guid.NewGuid();

            // 2. Act
            var response = await _client.GetAsync($"/api/decks/GetDeckById/{fakeId}");

            // 3. Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}