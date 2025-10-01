using Application.Common.Results;
using Application.Student.Commands.CreateStudent;
using Application.Student.Commands.DeleteStudent;
using Application.Student.Commands.UpdateStudent;
using Application.Student.Commands.UpdateStudentDetails;
using Application.Student.Dtos;
using Application.Student.Queries.GetAllStudents;
using Application.Student.Queries.GetStudentById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Student.Commands;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize(Roles = "Admin")]
    public sealed class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StudentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllStudentsQuery(), ct);
            return Ok(result.Value);
        }

        [HttpGet("GetStudentById/{id:guid}")]
        public async Task<IActionResult> GetStudentById(Guid id, CancellationToken ct) 
        {
            var query = new GetStudentByIdQuery(id);
            var result = await _mediator.Send(query, ct);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        // POST /api/students
        [HttpPost("CreateStudent")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken ct)
        {
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
    
            return CreatedAtAction(nameof(GetStudentById), new { id = result.Value!.Id }, result.Value);
        }

        // PUT /api/students/{id}
        [HttpPut("UpdateStudent/{id:guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentCommand command, CancellationToken ct)
        {
            if (id != command.Id)
            {
                return BadRequest(Error.Validation("Id.Mismatch", "ID i URL matchar inte ID i request body."));
            }

            var result = await _mediator.Send(command, ct);
            
            // FIX: Lade till komplett felhantering
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

        [HttpPatch("UpdateStudentDetails/{id:guid}")]
        public async Task<IActionResult> UpdateStudentDetails(Guid id, [FromBody] JsonPatchDocument<UpdateStudentDetailsDto> patchDoc, CancellationToken ct)
        {
            var command = new UpdateStudentDetailsCommand(id, patchDoc);
            var result = await _mediator.Send(command, ct);

            // FIX: Lade till komplett felhantering
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

        [HttpDelete("DeleteStudent/{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id, CancellationToken ct)
        {
            var command = new DeleteStudentCommand(id);
            var result = await _mediator.Send(command, ct);
            
            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    _ => BadRequest(result.Error)
                };
            }
                
            return NoContent();
        }
    }
}