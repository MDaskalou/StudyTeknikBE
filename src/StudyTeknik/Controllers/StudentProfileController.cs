using System.Security.Claims;
using Application.Common.Results;
using Application.StudentProfile.Queries.GetAllStudentProfile;
using Application.StudentProfiles.Commands.CreateStudentProfile;
using Application.StudentProfiles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// Om du vill kräva inloggning


namespace StudyTeknik.Controllers 
{
    [ApiController]
    [Route("api/student-profiles")]
    [Authorize]
    
    public class StudentProfilesController : ControllerBase
    {
        private readonly ISender _sender;

        public StudentProfilesController(ISender sender)
        {
            _sender = sender;
        }

        // --- 1. GET: Hämta alla profiler ---
        // URL: GET api/student-profiles
        [HttpGet("GetAllStudentProfiles")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var query = new GetAllStudentProfilesQuery();
            
            var result = await _sender.Send(query, ct);

            if (result.IsFailure)
            {
                return HandleFailure(result.Error);
            }

            return Ok(result.Value);
        }

        // --- 2. POST: Skapa profil ---
        // URL: POST api/student-profiles
        [HttpPost] 
        public async Task<IActionResult> Create([FromBody] CreateStudentProfileDto request, CancellationToken ct)
        {
            // A. Hämta User ID från token (Claims)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Safety check: Om ingen är inloggad eller ID saknas
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var studentId))
            {
                return Unauthorized(new { message = "Kunde inte identifiera användaren." });
            }

            // B. Skapa Command
            var command = new CreateStudentProfileCommand(
                studentId,
                request.PlanningHorizonWeeks,
                request.WakeUpTime,
                request.BedTime
            );

            // C. Skicka via MediatR
            var result = await _sender.Send(command, ct);

            // D. Hantera svar
            if (result.IsFailure)
            {
                return HandleFailure(result.Error);
            }

            // Returnera 200 OK (eller 201 Created om du vill vara strikt REST)
            return Ok(result.Value);
        }

        // --- Gemensam felhanterare ---
        private IActionResult HandleFailure(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => BadRequest(error),
                ErrorType.NotFound => NotFound(error),
                ErrorType.Conflict => Conflict(error),
                ErrorType.Forbidden => Forbid(error.Description),
                _ => StatusCode(500, error)
            };
        }
    }
}