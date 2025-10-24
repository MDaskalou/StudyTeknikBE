using Application.Common.Results;
using Application.Decks.Dtos;
using MediatR;

namespace Application.Decks.Commands.UpdateDecks
{
    public record UpdateDeckCommand(string Title, string CourseName, string SubjectName, Guid DeckId) :
        IRequest<OperationResult>;
}