using System.Net;
using System.Net.Http.Json;

namespace StudyTeknik.Test.DeckTests
{
    // DTO för att skicka/ta emot data i testet
    public record CreateDeckDto(string Title, string Description);
    public record DeckResponseDto(Guid Id, string Title, string Description);

    [TestFixture]
    public class DeckIntegrationTests
    {
        private CustomWebApplicationFactory.CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            // Starta upp servern och skapa en klient
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
            // 1. Arrange (Förbered data)
            var newDeck = new CreateDeckDto("Integration Test Deck", "Testing Create");

            // 2. Act (Skicka POST-anrop)
            var response = await _client.PostAsJsonAsync("/api/decks/CreateDeck", newDeck);

            // 3. Assert (Kolla att vi fick svar 200 eller 201)
            response.EnsureSuccessStatusCode(); // Kraschar om status är 400/500
            
            var createdDeck = await response.Content.ReadFromJsonAsync<DeckResponseDto>();
            Assert.That(createdDeck, Is.Not.Null);
            Assert.That(createdDeck.Title, Is.EqualTo("Integration Test Deck"));
        }

        [Test]
        public async Task GetAllDecks_ShouldReturnList_WhenDecksExist()
        {
            // 1. Arrange - Skapa en kortlek först så listan inte är tom
            var newDeck = new CreateDeckDto("List Test", "Testing List");
            await _client.PostAsJsonAsync("/api/decks", newDeck);

            // 2. Act - Hämta listan
            var response = await _client.GetAsync("/api/decks/GetAllDecks");

            // 3. Assert
            response.EnsureSuccessStatusCode();
            var decks = await response.Content.ReadFromJsonAsync<List<DeckResponseDto>>();
            
            Assert.That(decks, Is.Not.Null);
            Assert.That(decks.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetDeck_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // 1. Arrange - Ett påhittat ID
            var fakeId = Guid.NewGuid();

            // 2. Act
            var response = await _client.GetAsync($"/api/decks/{fakeId}");

            // 3. Assert - Här förväntar vi oss 404 Not Found
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}