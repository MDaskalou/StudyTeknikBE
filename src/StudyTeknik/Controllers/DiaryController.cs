using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Commands.CreateDiary;
using Application.Diary.Commands.UpdateDiary;
using Application.Diary.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/diary")]
    [Authorize(Roles = "Student")]

    public sealed class DiaryController : ControllerBase
    {
        private readonly IMediator _mediator; 
        public DiaryController(IMediator mediator) => _mediator = mediator;

        [HttpPost("CreateDiary")]
        public async Task<IActionResult> CreateDiaryEntry([FromBody]  CreateDiaryRequestDto requestDto, CancellationToken ct)
        {
            var command = new CreateDiaryCommand(requestDto.EntryDate, requestDto.Text);
    
            // STEG 2: Skicka det interna Command-objektet till MediatR
            var result = await _mediator.Send(command, ct);

            // STEG 3: Hantera resultatet (denna logik är densamma som förut)
            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return Created($"/api/diaries/{result.Value!.Id}", result.Value);

        }

        [HttpPut("UpdateDiary")]
        public async Task<IActionResult> UpdateDiary(Guid Id, [FromBody] UpdateDiaryDto dto, CancellationToken ct)
        {
            var command = new UpdateDiaryCommand(Id, dto.Text);
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
            
    }
}