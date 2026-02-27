using System.Security.Claims;
using Application.Common.Results;
using Application.StudentProfile.Queries.GetAllStudentProfile;
using Application.StudentProfiles.Commands.CreateStudentProfile;
using Application.StudentProfiles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyTeknik.Extensions;


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
        [HttpPost("CreateStudentProfile")] 
        public async Task<IActionResult> Create([FromBody] CreateStudentProfileDto request, CancellationToken ct)
        {
            // Hämta User ID från token via InternalUserId claim (satt av UserProvisioningMiddleware)
            var studentId = User.GetUserIdAsGuid();
            
            // Safety check: Om ID saknas returnera Unauthorized
            if (studentId == null || studentId == Guid.Empty)
            {
                return Unauthorized(new { 
                    message = "Kunde inte identifiera användaren. Ingen giltigt user ID hittades i token."
                });
            }

            // Skapa Command
            var command = new CreateStudentProfileCommand(
                studentId.Value,
                request.PlanningHorizonWeeks,
                request.WakeUpTime,
                request.BedTime
            );

            // Skicka via MediatR
            var result = await _sender.Send(command, ct);

            // Hantera svar
            if (result.IsFailure)
            {
                return HandleFailure(result.Error);
            }

            // Returnera 201 Created (REST best practice)
            return CreatedAtAction(nameof(GetAll), new { id = result.Value }, result.Value);
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