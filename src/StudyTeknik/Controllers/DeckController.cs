using Application.Common.Results;
using Application.Decks.Commands.CreateDeck;
using Application.Decks.Commands.DeleteDecks;
using Application.Decks.Commands.UpdateDecks;
using Application.Decks.Commands.UpdateDetailsDeck;
using Application.Decks.Dtos;
using Application.Decks.Queries.GetAllDecks;
using Application.Decks.Queries.GetDeckById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet]
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
        
        [HttpGet("GetDeckById/{Id}")]
        public async Task<IActionResult> GetDeckById(Guid Id, CancellationToken ct)
        {
            var query = new GetDeckByIdQuery(Id);
            
            var queryResult =  await _mediator.Send(query, ct);

            if (queryResult.IsFailure)
            {
                return queryResult.Error.Type switch 
                {
                    ErrorType.Validation => BadRequest(queryResult.Error),
                    ErrorType.NotFound => NotFound(queryResult.Error),
                    ErrorType.Forbidden => Forbid(),
                    ErrorType.Conflict => Conflict(queryResult.Error),
                    _ => BadRequest(queryResult.Error) 
                };
            }
            return Ok(queryResult.Value);
        }

        [HttpPut("UpdateDeck/{Id}")]
        public async Task<IActionResult> UpdateDeck([FromBody] UpdateDeckCommand command, CancellationToken ct)
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
            return NoContent();
        }

        [HttpPatch("UpdateDeckDetails/{id}")]
        public async Task<IActionResult> PatchDeck(Guid id, [FromBody] JsonPatchDocument<UpdateDetailsDeckDto>
            patchDoc, CancellationToken ct)
        {
            var command = new UpdateDetailsDeckCommand(id, patchDoc);
            var result = await _mediator.Send(command, ct);
            
            if(result.IsFailure)
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
            
                return NoContent();
            
        }
        
        [HttpDelete("DeleteDeck/{Id}")]
        public async Task<IActionResult> DeleteDeck(Guid Id, CancellationToken ct)
        {
            var command = new DeleteDecksCommand(Id);
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
            return NoContent();
        }
    }
    
}