using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [Authorize(Policy = "MentorOnly")]
    [ApiController]
    [Route("api/mentor")]
    public sealed class MentorController : ControllerBase
    {
        private readonly ICurrentUserService _current;
        private readonly IMentorRepository _mentors;

        public MentorController(ICurrentUserService current, IMentorRepository mentors)
            => (_current, _mentors) = (current, mentors);

        [HttpGet("mentees")]
        public async Task<IActionResult> GetMentees(CancellationToken ct)
        {
            if (!_current.UserId.HasValue) return Unauthorized();
            var list = await _mentors.GetMenteesAsync(_current.UserId.Value, ct);
            return Ok(list);
        }
    }
}