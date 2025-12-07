using System.Security.Claims;
using Infrastructure.Service;
using Microsoft.AspNetCore.Http;
using Moq;

namespace StudyTeknik.Test.Services
{
    [TestFixture]
    public class CurrentUserServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private CurrentUserService _sut; // SUT = System Under Test

        [SetUp]
        public void Setup()
        {
            // Vi nollställer mocken inför varje test
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _sut = new CurrentUserService(_httpContextAccessorMock.Object);
        }

        // --- Tester för UserId (Intern state) ---

        [Test]
        public void SetUserId_ShouldSetId_WhenIdIsNull()
        {
            // Arrange
            var newId = Guid.NewGuid();

            // Act
            _sut.SetUserId(newId);

            // Assert
            Assert.That(_sut.UserId, Is.EqualTo(newId));
        }

        [Test]
        public void SetUserId_ShouldNotOverwrite_WhenIdIsAlreadySet()
        {
            // Arrange
            var initialId = Guid.NewGuid();
            var newId = Guid.NewGuid();
            _sut.SetUserId(initialId);

            // Act
            _sut.SetUserId(newId); // Försöker skriva över

            // Assert
            Assert.That(_sut.UserId, Is.EqualTo(initialId), "UserId borde inte ha ändrats.");
        }

        // --- Tester för ExternalId (Claims) ---

        [Test]
        public void ExternalId_ShouldReturnSubClaim_WhenUserIsAuthenticated()
        {
            // Arrange
            var expectedExternalId = "google-auth-123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", expectedExternalId)
            }));

            // Vi mockar att HttpContext har denna user
            var context = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _sut.ExternalId;

            // Assert
            Assert.That(result, Is.EqualTo(expectedExternalId));
        }

        [Test]
        public void ExternalId_ShouldReturnNull_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

            // Act
            var result = _sut.ExternalId;

            // Assert
            Assert.That(result, Is.Null);
        }

        // --- Tester för RoleName (Claims) ---

        [Test]
        public void RoleName_ShouldReturnRole_WhenClaimTypeIsRole()
        {
            // Arrange
            var expectedRole = "Admin";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, expectedRole)
            }));

            var context = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _sut.RoleName;

            // Assert
            Assert.That(result, Is.EqualTo(expectedRole));
        }

        [Test]
        public void RoleName_ShouldReturnRole_WhenClaimTypeIsCustomString()
        {
            // Arrange - Testar fallback-logiken "role"
            var expectedRole = "Student";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("role", expectedRole)
            }));

            var context = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            // Act
            var result = _sut.RoleName;

            // Assert
            Assert.That(result, Is.EqualTo(expectedRole));
        }
    }
}