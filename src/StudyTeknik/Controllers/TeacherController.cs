using Application.Common.Results;
using Application.Teacher.Commands.CreateTeacher;
using Application.Teacher.Commands.DeleteTeacher;
using Application.Teacher.Commands.UpdateTeacher;
using Application.Teacher.Commands.UpdateTeacherDetails;
using Application.Teacher.Dtos;
using Application.Teacher.Queries.GetAllTeachers;
using Application.Teacher.Queries.GetTeacherById;
using Application.Teacher.Repository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controller
{
    [ApiController]
    [Route("api/teachers")]
    [Authorize(Roles = "Admin")]

    public sealed class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController( IMediator mediator)
        {
            _mediator = mediator;
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
            
            return Ok(result.Value);
        }
        
        [HttpPost("CreateTeacher")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
                return result.Error.Type switch
                {
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            
            return CreatedAtAction(nameof(GetTeacherById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("UpdateTeacher/{id:guid}")]
        public async Task<IActionResult> UpdateTeacher(Guid id, [FromBody] UpdateTeacherCommand command,
            CancellationToken ct)
        {
            if (id != command.Id)
            {
                var error = Error.Validation("Id.Mismatch", "ID i URL matchar inte ID i request body.");
                return BadRequest(error);
            }

            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return NoContent();
        }
        [HttpPatch("UpdateTeacher/{id:guid}")]
        public async Task<IActionResult> UpdateTeacherDetails(Guid id, [FromBody] 
            JsonPatchDocument<UpdateTeacherDetailsDto> patchDoc, CancellationToken ct)
        {
            var command = new UpdateTeacherDetailsCommand(id, patchDoc);
            
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            }
            return NoContent();
        }

        [HttpDelete("DeleteTeacher/{id:guid}")]
        public async Task<IActionResult> DeleteTeacher(Guid id, CancellationToken ct)
        {
            var command = new DeleteTeacherCommand(id);
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return result.Error.Type == ErrorType.NotFound
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);
            }

            return NoContent();
        }
    }
}