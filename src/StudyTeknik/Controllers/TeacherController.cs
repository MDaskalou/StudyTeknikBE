using Application.Teacher.Dtos;
using Application.Teacher.Queries.GetAllTeachers;
using Application.Teacher.Queries.GetTeacherById;
using Application.Teacher.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controller
{
    [ApiController]
    [Route("api/teacher")]
    [Authorize(Roles = "Admin")]

    public sealed class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMediator _mediator;

        public TeacherController(ITeacherRepository teacherRepository, IMediator mediator)
        {
            _mediator = mediator;
            _teacherRepository = teacherRepository;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTeacherById(Guid id, CancellationToken ct)
        {
            var query = new GetTeacherByIdQuery(id);
            
            var result = await _mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpGet("GetAllTeachers")]
        public async Task<IActionResult> GetAllTachers(CancellationToken ct)
        {
            var query = new GetAllTeachersQuery();
            
            var result = await _mediator.Send(query, ct);
            
            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }
            return Ok(result.Value);
        }
    }
}