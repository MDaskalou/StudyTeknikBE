using System.Security.Claims;
using Application.Common.Results;
using Application.StudentProfile.Commands.CreateStudentProfile;
using Application.StudentProfile.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
// Ditt namespace för Command
// För Result och ErrorType
// För att hitta inloggad User

// <--- Lägg till denna using

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/student-profiles")]
    public class StudentProfilesController : ControllerBase
    {
        private readonly ISender _sender;

        public StudentProfilesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("createstudentprofile")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateStudentProfileDto request)
        {
            // 1. Hämta User ID från token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Validering att vi faktiskt fick ett ID (Auth middleware borde sköta detta, men safety first)
            if (!Guid.TryParse(userIdString, out var studentId))
            {
                return Unauthorized("Kunde inte identifiera användaren.");
            }

            // 2. Mappa DTO -> Command
            // Här gör vi om den "dumma" DTO:n till vårt interna Command
            var command = new CreateStudentProfileCommand(
                studentId,
                request.PlanningHorizonWeeks,
                request.WakeUpTime,
                request.BedTime
            );

            // 3. Skicka iväg
            var result = await _sender.Send(command);

            // 4. Hantera svar
            if (result.IsFailure)
            {
                return HandleFailure(result.Error);
            }

            // Returnera 200 OK med ID:t
            return Ok(result.Value);
        }

        private IActionResult HandleFailure(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => BadRequest(error),
                ErrorType.NotFound => NotFound(error),
                ErrorType.Conflict => Conflict(error),
                _ => StatusCode(500, error)
            };
        }
    }
}