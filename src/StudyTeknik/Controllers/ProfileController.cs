using Application.Student.Queries.GetMyGeneralInfo;
using Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/profile")] // Eller "api/me"
    [Authorize] // <--- Här krävs bara inloggning, ingen specifik roll!
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Get-My-Profile")] // URL blir: api/profile
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetStudentGeneralInfoQuery(), ct);

            if (result.IsFailure)
            {
                return result.Error.Type == ErrorType.NotFound 
                    ? NotFound(result.Error) 
                    : BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}