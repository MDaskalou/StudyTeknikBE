using Application.Common.Results;
using Application.FlashCards.Dtos;
using MediatR;

namespace Application.FlashCards.Queries.GetFlashCardById
{
    public sealed record GetFlashCardByIdQuery(
        Guid FlashCardId,
        Guid DeckId
    ) : IRequest<OperationResult<FlashCardDto>>;
}

