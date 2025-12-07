using Infrastructure.Service;

namespace StudyTeknik.Test.Services
{
    [TestFixture]
    public class DateTimeProviderTests
    {
        [Test]
        public void UtcNow_ShouldReturnCurrentDate()
        {
            // Arrange
            var provider = new DateTimeProvider();

            // Act
            var result = provider.UtcNow;

            // Assert
            // Vi kollar att tiden är "nu" (inom 1 sekunds marginal)
            Assert.That(result, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        }
    }
}