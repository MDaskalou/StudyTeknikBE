using Application.Common.Results;
using Application.Decks.Dtos;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Decks.Commands.UpdateDetailsDeck
{
    public record UpdateDetailsDeckCommand(Guid DeckId, JsonPatchDocument<UpdateDetailsDeckDto> PatchDoc) :
        IRequest<OperationResult>;
}