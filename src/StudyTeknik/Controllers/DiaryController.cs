using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Commands.CreateDiary;
using Application.Diary.Commands.DeleteDiary;
using Application.Diary.Commands.UpdateDiary;
using Application.Diary.Commands.UpdateDiaryDetails;
using Application.Diary.Dtos;
using Application.Diary.Queries.GetAllDiary;
using Application.Diary.Queries.GetDiaryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/diary")]

    public sealed class DiaryController : ControllerBase
    {
        private readonly IMediator _mediator; 
        public DiaryController(IMediator mediator) => _mediator = mediator;

        [HttpPost("CreateDiary")]
        [Authorize(Policy = "HasWriteScope")]
        

        [HttpPut("UpdateDiary/{Id:guid}")]
        [Authorize(Policy = "HasWriteScope")]

        public async Task<IActionResult> UpdateDiary(Guid id, [FromBody] UpdateDiaryDto dto, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Kunde inte identifiera användaren" });
            }
            
            var command = new UpdateDiaryCommand(id,userId, dto.Text);
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    _ => BadRequest()
                };
            }
            return NoContent();

        }
        
        [HttpPatch("UpdateDiaryDetails/{id:guid}")]
        [Authorize(Policy = "HasWriteScope")]

        
        public async Task<IActionResult> UpdateDiaryDetails(Guid id, [FromBody] JsonPatchDocument<UpdateDiaryDetailsDto> patchDoc, CancellationToken ct)
        {
            var command = new UpdateDiaryDetailsCommand(id, patchDoc);
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

        [HttpDelete("DeleteDiary/{id:guid}")]
        [Authorize(Policy = "HasWriteScope")]


        public async Task<IActionResult> DeleteDiary(Guid id, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Kunde inte identifiera användaren" });
            }
            
            var command = new DeleteDiaryCommand(id, userId); 
            
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

        [HttpGet("GetAllDiariesForStudent")]
        [Authorize(Policy = "HasReadScope")]
        public async Task<IActionResult> GetAllDiariesForStudent(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Kunde inte identifiera användaren från token." });
            }

            var query = new GetAllDiaryQuery(userId);
            var result = await _mediator.Send(query, ct);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("GetGiaryById/{id:guid}")]
        [Authorize(Policy = "HasReadScope")]
        public async Task<IActionResult> GetGiaryById(Guid id, CancellationToken ct)
        {
            var query = new GetDiaryByIdQuery(id);
            var result = await _mediator.Send(query, ct);
            
            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    _ => BadRequest(result.Error)
                };
            }
            
            return Ok(result.Value);
        }
    }
}