using Application.Common.Results;
using Application.FlashCards.Dtos;
using MediatR;

namespace Application.FlashCards.Queries.GetAllFlashCardsForDeckQuery
{
    public record GetAllFlashCardsForDeckQuery(Guid DeckId) 
        : IRequest<OperationResult<List<FlashCardDto>>>;
}