using Application.StudentProfile.Queries.GetAllStudentProfile;
using Application.StudentProfiles.IRepository;
using Application.StudentProfiles.Queries.GetAllStudentProfile;
using Domain.Models.StudentProfiles;
using FluentAssertions;
using NSubstitute;
// Din domänmodell

namespace StudyTeknik.Test.StudentProfileTest
{
    [TestFixture]
    public class GetAllStudentProfilesHandlerTests
    {
        private IStudentProfileRepository _repositoryMock;
        private GetAllStudentProfilesHandler _handler;

        [SetUp] // Körs innan varje test
        public void SetUp()
        {
            // Vi skapar en "fejk" version av repositoryt
            _repositoryMock = Substitute.For<IStudentProfileRepository>();
            _handler = new GetAllStudentProfilesHandler(_repositoryMock);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_WhenProfilesExist()
        {
            // Arrange (Förbered)
            var studentId = Guid.NewGuid();
            
            // Skapa en fake domän-profil med din factory/konstruktor
            var domainProfile = new StudentProfile(
                studentId, 
                planningWeeks: 2, 
                wakeUp: TimeSpan.FromHours(7), 
                bedTime: TimeSpan.FromHours(22)
            );

            var domainList = new List<StudentProfile> { domainProfile };

            // Säg åt mocken: "När GetAllAsync anropas, returnera vår lista"
            _repositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(domainList);

            var query = new GetAllStudentProfilesQuery();

            // Act (Utför)
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert (Kontrollera)
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(1);
            
            // Kontrollera att mappningen blev rätt
            result.Value.First().StudentId.Should().Be(studentId);
            result.Value.First().PlanningHorizonWeeks.Should().Be(2);
        }

        [Test]
        public async Task Handle_Should_ReturnEmptyList_WhenNoProfilesExist()
        {
            // Arrange
            _repositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(new List<StudentProfile>()); // Tom lista

            // Act
            var result = await _handler.Handle(new GetAllStudentProfilesQuery(), CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}