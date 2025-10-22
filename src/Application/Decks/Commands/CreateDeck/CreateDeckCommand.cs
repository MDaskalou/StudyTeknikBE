using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository;
using MediatR;

namespace Application.Decks.Commands.CreateDeck
{
    public record CreateDeckCommand(string Title, string CourseName, string SubjectName) :
        IRequest<OperationResult<DeckDto>>;
}