using Application.Common.Results;
using Application.Student.Commands;
using Application.Student.Commands.CreateStudent;
using Application.Student.Commands.UpdateStudent;
using Application.Student.Dtos;
 using Application.Student.Queries.GetAllStudents;
 using Application.Student.Queries.GetStudentById;
 using MediatR;
 using Microsoft.AspNetCore.Authorization;
 using Microsoft.AspNetCore.JsonPatch;
 using Microsoft.AspNetCore.Mvc;
 
 namespace StudyTeknik.Controllers
 {
     [ApiController]
     [Route("api/students")]
     [Authorize(Roles = "Admin")]
     public sealed class StudentsController : ControllerBase
     {
         private readonly IMediator _mediator;
         public StudentsController(IMediator mediator) => _mediator = mediator;
 
         [HttpGet] 
         public async Task<IActionResult> GetAllStudents(CancellationToken ct)
         {
             var student = await _mediator.Send(new GetAllStudentsQuery(), ct);
             return Ok(student);
         }
 
      
 
         [HttpGet("{id:guid}")] // FIX 4: Rätt HTTP-metod och route
         public async Task<IActionResult> GetStudentById(Guid id, CancellationToken ct) // Bättre namn
         {
             var query = new GetStudentByIdQuery(id);
             var result = await _mediator.Send(query, ct);

             if (result.IsFailure)
             {
                 // FIX 5: Konsekvent felhantering med ErrorType
                 return result.Error.Type switch
                 {
                     ErrorType.NotFound => NotFound(result.Error),
                     _ => BadRequest(result.Error)
                 };
             }
             return Ok(result.Value);
         }

         [HttpPost]
         public async Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken ct)
         {
             var result = await _mediator.Send(command, ct);

             if (result.IsFailure)
             {
                 return result.Error.Type switch
                 {
                     ErrorType.NotFound => NotFound(result.Error),
                     ErrorType.Conflict => Conflict(result.Error),
                     ErrorType.Validation => BadRequest(result.Error),
                     _ => BadRequest(result.Error)
                 };
             }
    
             return CreatedAtAction(nameof(GetStudentById), new { id = result.Value!.Id }, result.Value);
         }

         [HttpPut("{id:guid}")]
         public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentCommand command,
             CancellationToken ct)
         {
             // Säkerställ att ID:t i URL:en matchar det i request body
             if (id != command.Id)
             {
                 // Vi kan skapa ett specifikt fel här för tydlighetens skull
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

             // Vid lyckad uppdatering, returnera 204 No Content
             return NoContent();
         }

         [HttpPatch("{id:guid}")]
             public async Task<IActionResult> UpdateStudentDetails(Guid id, [FromBody] JsonPatchDocument<UpdateStudentDetailsDto> patchDoc, CancellationToken ct)
             {
                 // Skapa kommandot med ID från URL:en och patch-dokumentet från body
                 var command = new UpdateStudentDetailsCommand(id, patchDoc);
    
                 var result = await _mediator.Send(command, ct);

                 if (result.IsFailure)
                 {
                     return result.Error.Type switch
                     {
                         ErrorType.NotFound => NotFound(result.Error),
                         ErrorType.Conflict => Conflict(result.Error),
                         ErrorType.Validation => BadRequest(result.Error),
                         _ => BadRequest(result.Error)
                     };
                 }

                 // Vid lyckad patch, returnera 204 No Content
                 return NoContent();
             }
     }
 
         
 }
 