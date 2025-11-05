
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.FlashCards.Commands.AddFlashcardToDeckCommand;
using Application.FlashCards.Commands.AddFlashcardToDeckCommand.AddFlashcardToDeckCommand;
using Application.FlashCards.Commands.UpdateFlashcardCommand; // För ditt command
using Application.FlashCards.Dtos;
using Application.FlashCards.Queries.GetAllFlashCardsForDeckQuery;
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
        
        [HttpGet] 
        public async Task<IActionResult> GetAllFlashCardsForDeck(Guid deckId, CancellationToken ct)
        {
            // 1. Skapa din nya query
            var query = new GetAllFlashCardsForDeckQuery(deckId);
        
            var result = await _mediator.Send(query, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value); 
            }
            
            return BadRequest(result.Error); 
        }
        
        
        [HttpPut("{flashCardId:guid}")] // Svarar på: PUT /api/decks/{deckId}/flashcards/{flashCardId}
        public async Task<IActionResult> UpdateFlashCard(
            [FromRoute] Guid deckId,
            [FromRoute] Guid flashCardId,
            [FromBody] UpdateFlashcardRequest request, // Använder din befintliga recor
            CancellationToken ct)
        {
            // 1. Skapa ditt nya command (matchar din Handler)
            var command = new UpdateFlashcardCommand(
                deckId,
                flashCardId,
                request.FrontText,
                request.BackText
            );

            var result = await _mediator.Send(command, ct);

            return result.IsSuccess
                ? NoContent() // Standard REST-svar för en lyckad PUT
                : BadRequest(result.Error);
        }
        

    }
    public record AddFlashCardRequest(string FrontText, string BackText);
    public record UpdateFlashcardRequest(string FrontText, string BackText);
}