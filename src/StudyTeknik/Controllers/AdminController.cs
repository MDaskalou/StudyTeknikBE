using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IClassRepository _classes;
        public AdminController(IClassRepository classes) => _classes = classes;

        [HttpGet("classes")]
        public async Task<IActionResult> GetAllClasses(CancellationToken ct)
            => Ok(await _classes.GetAllAsync(ct));

        public sealed record CreateStudentDto(string FirstName, string LastName, string Email);

        [HttpPost("students")]
        public IActionResult CreateStudent([FromBody] CreateStudentDto dto)
        {
            // TODO: implementera riktig skap-logik via domän + repos
            return StatusCode(201);
        }
    }
}