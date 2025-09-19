using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [Authorize(Policy = "StudentOnly")]
    [ApiController]
    [Route("api/diary")]
    public sealed class DiaryController : ControllerBase
    {
        private readonly ICurrentUserService _current;
        private readonly IDiaryRepository _diaries;

        public DiaryController(ICurrentUserService current, IDiaryRepository diaries)
            => (_current, _diaries) = (current, diaries);

        [HttpGet("my")]
        public async Task<IActionResult> GetMyEntries(CancellationToken ct)
        {
            if (!_current.UserId.HasValue) return Unauthorized();
            var list = await _diaries.GetByStudentAsync(_current.UserId.Value, ct);
            return Ok(list);
        }
    }
}