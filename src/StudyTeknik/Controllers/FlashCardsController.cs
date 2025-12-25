
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.FlashCards.Commands;
using Application.FlashCards.Commands.DeleteFlashCard;
using Application.FlashCards.Commands.UpdateFlashCard;
using Application.FlashCards.Dtos;
using Application.FlashCards.Queries.GetAllFlashCardsForDeckQuery;
using Application.FlashCards.Queries.GetFlashCardById;
using Application.Common.Results;
using Microsoft.AspNetCore.Authorization;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/decks/{deckId:guid}/flashcards")]
    public class FlashCardController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public FlashCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Skapa en ny flashcard i ett deck
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddFlashCard(
            [FromRoute] Guid deckId,
            [FromBody] AddFlashCardRequest request, 
            CancellationToken ct)
        {
            var command = new AddFlashcardToDeckCommand(
                deckId,
                request.FrontText,
                request.BackText
            );
            
            var result = await _mediator.Send(command, ct);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetFlashCardById), new { deckId, flashCardId = result.Value!.Id }, result.Value);
            }
            
            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.NotFound => NotFound(result.Error),
                ErrorType.Forbidden => Forbid(),
                _ => StatusCode(500, result.Error)
            };
        }
        
        /// <summary>
        /// Hämta alla flashcards för ett deck
        /// </summary>
        [HttpGet] 
        public async Task<IActionResult> GetAllFlashCardsForDeck(
            [FromRoute] Guid deckId, 
            CancellationToken ct)
        {
            var query = new GetAllFlashCardsForDeckQuery(deckId);
            var result = await _mediator.Send(query, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value); 
            }
            
            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.NotFound => NotFound(result.Error),
                _ => StatusCode(500, result.Error)
            };
        }

        /// <summary>
        /// Hämta en enskild flashcard
        /// </summary>
        [HttpGet("{flashCardId:guid}")]
        public async Task<IActionResult> GetFlashCardById(
            [FromRoute] Guid deckId,
            [FromRoute] Guid flashCardId,
            CancellationToken ct)
        {
            var query = new GetFlashCardByIdQuery(flashCardId, deckId);
            var result = await _mediator.Send(query, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.NotFound => NotFound(result.Error),
                _ => StatusCode(500, result.Error)
            };
        }

        /// <summary>
        /// Uppdatera en flashcard
        /// </summary>
        [HttpPut("{flashCardId:guid}")]
        public async Task<IActionResult> UpdateFlashCard(
            [FromRoute] Guid deckId,
            [FromRoute] Guid flashCardId,
            [FromBody] UpdateFlashCardRequest request,
            CancellationToken ct)
        {
            var command = new UpdateFlashCardCommand(
                flashCardId,
                deckId,
                request.FrontText,
                request.BackText
            );

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.NotFound => NotFound(result.Error),
                _ => StatusCode(500, result.Error)
            };
        }

        /// <summary>
        /// Ta bort en flashcard
        /// </summary>
        [HttpDelete("{flashCardId:guid}")]
        public async Task<IActionResult> DeleteFlashCard(
            [FromRoute] Guid deckId,
            [FromRoute] Guid flashCardId,
            CancellationToken ct)
        {
            var command = new DeleteFlashCardCommand(flashCardId, deckId);
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return result.Error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Error),
                ErrorType.NotFound => NotFound(result.Error),
                _ => StatusCode(500, result.Error)
            };
        }
    }

    public record AddFlashCardRequest(string FrontText, string BackText);
    public record UpdateFlashCardRequest(string FrontText, string BackText);
}