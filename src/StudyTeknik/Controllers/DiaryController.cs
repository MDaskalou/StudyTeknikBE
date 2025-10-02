using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Commands.CreateDiary;
using Application.Diary.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/diary")]
    [Authorize]

    public sealed class DiaryController : ControllerBase
    {
        private readonly IMediator _mediator; 
        public DiaryController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize(Roles = "Student")] 
        public async Task<IActionResult> CreateDiaryEntry([FromBody]  CreateDiaryCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Conflict => Conflict(result.Error),
                    ErrorType.Validation => BadRequest(result.Error),
                    _ => BadRequest()
                };
            }
            
            return Created($"/api/diaries/{result.Value!.Id}", result.Value);

        }
            
        
    }
}