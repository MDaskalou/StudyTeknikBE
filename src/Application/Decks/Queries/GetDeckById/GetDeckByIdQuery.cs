using Application.Common.Results;
using Application.Decks.Dtos;
using Domain.Models.Flashcards;
using MediatR;

namespace Application.Decks.Queries.GetDeckById
{
    public record GetDeckByIdQuery(Guid DeckId): IRequest<OperationResult<DeckDto>>;
}