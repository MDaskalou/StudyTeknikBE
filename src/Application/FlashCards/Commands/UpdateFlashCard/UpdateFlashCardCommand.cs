using Application.Common.Results;
using Application.FlashCards.Dtos;
using MediatR;

namespace Application.FlashCards.Commands.UpdateFlashCard
{
    public sealed record UpdateFlashCardCommand(
        Guid FlashCardId,
        Guid DeckId,
        string FrontText,
        string BackText
    ) : IRequest<OperationResult<FlashCardDto>>;
}

