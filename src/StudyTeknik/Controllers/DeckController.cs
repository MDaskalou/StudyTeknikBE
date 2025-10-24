using Application.Common.Results;
using Application.Decks.Commands.CreateDeck;
using Application.Decks.Queries.GetAllDecks;
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

        [HttpGet("GetAllDecks")]
        public async Task<IActionResult> GetAllDecks(CancellationToken ct)
        {
            var query = new GetAllDecksQuery();
            
            var queryResult =  await _mediator.Send(query, ct);

            if (queryResult.IsFailure)
            {
                return queryResult.Error.Type switch
                {

                    ErrorType.Forbidden => Forbid(),
                    _ => BadRequest(queryResult.Error)

                };
            }
            return Ok(queryResult.Value);
        }
        [HttpGet("GetDeckById/{id}")]
        public async Task<IActionResult> GetDeckById(Guid id, CancellationToken ct)
        {
            // TODO: Implementera GetDeckByIdQuery och Handler härnäst
            await Task.Delay(10, ct); // Dummy await
            return NotFound(new Error("NotImplemented", $"GetDeckById {id} är inte implementerad än.", ErrorType.NotFound));
        }
    }
    
}