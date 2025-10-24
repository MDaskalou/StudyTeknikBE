using Application.Common.Results;
using Application.Decks.Dtos;
using MediatR;

namespace Application.Decks.Queries.GetAllDecks
{
    public sealed record GetAllDecksQuery : IRequest<OperationResult<List<DeckDto>>>;

}