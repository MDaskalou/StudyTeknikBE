
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.FlashCards.Commands; // För ditt command
using Application.FlashCards.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize] // Säkra dina endpoints!
    [Route("api/decks/{deckId:guid}/flashcards")]

    public class FlashCardController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public FlashCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddFlashCard([FromRoute] Guid deckId,
            [FromBody] AddFlashCardRequest request, CancellationToken ct)
        {
            var command = new AddFlashcardToDeckCommand(
                deckId,
                request.FrontText,
                request.BackText
            );
            
            var result = await _mediator.Send(command, ct);
            
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }
        

    }
    public record AddFlashCardRequest(string FrontText, string BackText);
    public record UpdateFlashcardRequest(string FrontText, string BackText);
}