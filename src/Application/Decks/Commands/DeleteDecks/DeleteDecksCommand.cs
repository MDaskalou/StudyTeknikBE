using Application.Common.Results;
using Application.Decks.IRepository;
using MediatR;

namespace Application.Decks.Commands.DeleteDecks
{
    public sealed record DeleteDecksCommand(Guid DeckId) : IRequest<OperationResult>;

}