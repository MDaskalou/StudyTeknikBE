using Infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Moq;

namespace StudyTeknik.Test.Services
{
    [TestFixture]
    public class AiServiceTests
    {
        [Test]
        public async Task RewriteDiaryEntryAsync_ShouldReturnOriginal_WhenTextIsEmpty()
        {
            // Arrange
            // Vi måste mocka config för att konstruktorn inte ska krascha
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["GoogleAI:ApiKey"]).Returns("fake-key");

            var service = new AIService(mockConfig.Object);
            var emptyText = "";

            // Act
            var result = await service.RewriteDiaryEntryAsync(emptyText, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(emptyText));
        }

        [Test]
        public async Task RewriteDiaryEntryAsync_ShouldReturnNull_WhenTextIsNull()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["GoogleAI:ApiKey"]).Returns("fake-key");

            var service = new AIService(mockConfig.Object);
            string nullText = null;

            // Act
            var result = await service.RewriteDiaryEntryAsync(nullText, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}