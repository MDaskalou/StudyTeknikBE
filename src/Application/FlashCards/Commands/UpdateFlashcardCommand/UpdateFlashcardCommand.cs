using Application.Common.Results;
using MediatR;

namespace Application.FlashCards.Commands.UpdateFlashcardCommand
{
    public record UpdateFlashcardCommand(
        Guid DeckId,
        Guid FlashCardId,
        string FrontText,
        string BackText
    ) : IRequest<OperationResult>; // Returnerar 'Unit' (tomt svar)
}