using Application.Common.Results;
using MediatR;

namespace Application.FlashCards.Commands.DeleteFlashCard
{
    public sealed record DeleteFlashCardCommand(
        Guid FlashCardId,
        Guid DeckId
    ) : IRequest<OperationResult>;
}

