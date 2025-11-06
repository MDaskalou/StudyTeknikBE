using Application.Common.Results;
using Application.FlashCards.Dtos;
using MediatR;

namespace Application.FlashCards.Commands
{
    public record AddFlashcardToDeckCommand(Guid DeckId,string FrontText, string BackText) 
        :IRequest<OperationResult<FlashCardDto>>;
}