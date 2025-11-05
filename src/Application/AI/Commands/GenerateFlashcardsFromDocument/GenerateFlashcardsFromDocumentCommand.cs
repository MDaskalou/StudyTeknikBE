using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;
// För AiGeneratedCardDto

namespace Application.AI.Commands.GenerateFlashcardsFromDocument
{
    public record GenerateFlashcardsFromDocumentCommand(
            Guid DeckId, 
            Stream FileStream, 
            string FileName) 
        : IRequest<OperationResult<List<AiGeneratedCardDto>>>; 
}