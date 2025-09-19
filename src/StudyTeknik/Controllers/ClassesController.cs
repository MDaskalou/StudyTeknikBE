using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [Authorize(Policy = "TeacherOnly")]
    [ApiController]
    [Route("api/classes")]
    public sealed class ClassesController : ControllerBase
    {
        private readonly ICurrentUserService _current;
        private readonly IClassRepository _classes;

        public ClassesController(ICurrentUserService current, IClassRepository classes)
            => (_current, _classes) = (current, classes);

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine(CancellationToken ct)
        {
            if (!_current.UserId.HasValue) return Unauthorized();
            var list = await _classes.GetByTeacherAsync(_current.UserId.Value, ct);
            return Ok(list);
        }
    }
}