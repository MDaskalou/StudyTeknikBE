using NUnit.Framework;       // För [TestFixture], [Test], [SetUp]
using NSubstitute;           // För Substitute.For, Returns, Received, Arg
using FluentAssertions;      // För .Should().Be...
using Domain.Common;         // För ErrorType
using Domain.Models.StudentProfiles; // För StudentProfile-objektet
using Application.StudentProfile.Commands.CreateStudentProfile;
using System.Threading;
using System.Threading.Tasks;
using System;
using Application.Common.Results;
using Application.StudentProfiles.Commands.CreateStudentProfile;
using Application.StudentProfiles.IRepository;

namespace StudyTeknik.Test.StudentProfileTest
{
    [TestFixture]
    public class CreateStudentProfileHandlerTests
    {
        private CreateStudentProfileHandler _handler;
        private IStudentProfileRepository _repositoryMock;

        [SetUp]
        public void Setup()
        {
            // 1. Skapa en mock av repositoryt
            _repositoryMock = Substitute.For<IStudentProfileRepository>();

            // 2. Skapa handlern och skicka in mocken
            _handler = new CreateStudentProfileHandler(_repositoryMock);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_WhenProfileDoesNotExist()
        {
            // Arrange (Förberedelser)
            var command = new CreateStudentProfileCommand(
                StudentId: Guid.NewGuid(),
                PlanningHorizonWeeks: 4,
                WakeUpTime: new TimeSpan(7, 0, 0),
                BedTime: new TimeSpan(22, 0, 0)
            );

            // Mocka beteendet: "När vi kollar om user finns, svara false"
            _repositoryMock
                .ExistsByUserIdAsync(command.StudentId, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act (Utförande)
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert (Verifiering)
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty(); // Vi ska ha fått ett GUID tillbaka

            // Kontrollera att AddAsync faktiskt anropades en gång med rätt data
            await _repositoryMock.Received(1).AddAsync(
                Arg.Is<Domain.Models.StudentProfiles.StudentProfile>(p => 
                    p.StudentId == command.StudentId && 
                    p.PlanningHorizonWeeks == command.PlanningHorizonWeeks), 
                Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public async Task Handle_Should_ReturnConflict_WhenProfileAlreadyExists()
        {
            // Arrange
            var command = new CreateStudentProfileCommand(
                StudentId: Guid.NewGuid(),
                PlanningHorizonWeeks: 4,
                WakeUpTime: TimeSpan.Zero,
                BedTime: TimeSpan.Zero
            );

            // Mocka beteendet: "När vi kollar om user finns, svara TRUE"
            _repositoryMock
                .ExistsByUserIdAsync(command.StudentId, Arg.Any<CancellationToken>())
                .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Conflict);
            result.Error.Code.Should().Be("StudentProfile.AlreadyExists");

            // Kontrollera att AddAsync ALDRIG anropades (vi vill inte spara dubbletter)
            await _repositoryMock.DidNotReceive().AddAsync(
                Arg.Any<Domain.Models.StudentProfiles.StudentProfile>(), 
                Arg.Any<CancellationToken>()
            );
        }
    }
}