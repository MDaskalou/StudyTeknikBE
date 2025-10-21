using Application.Common.Results;
using Application.Decks.Commands.CreateDeck;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/decks")]
    [Authorize]

    public sealed class DeckController : ControllerBase
    {
        private readonly IMediator _mediator; 
        
        public DeckController(IMediator mediator) => _mediator = mediator;

        [HttpPost("CreateDeck")]
        public async Task<IActionResult> CreateDeck([FromBody] CreateDeckCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Validation => BadRequest(result.Error),
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Forbidden => Forbid(), 
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error) 
                };
            }
            return Created($"/api/decks/{result.Value!.Id}", result.Value);
            
        }
    }
    
}