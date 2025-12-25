using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;

namespace Application.AI.Commands.GenerateFlashCards
{
    public sealed record GenerateFlashCardsCommand(
        string PdfContent,
        Guid DeckId
    ) : IRequest<OperationResult<GenerateFlashCardsResponseDto>>;
}

