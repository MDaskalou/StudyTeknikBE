using Application.Common.Results;
using Application.Diary.Dtos;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Diary.Commands.UpdateDiaryDetails
{
    public sealed record UpdateDiaryDetailsCommand(
        Guid Id,
        JsonPatchDocument<UpdateDiaryDetailsDto> PatchDoc) : IRequest<OperationResult>;



}