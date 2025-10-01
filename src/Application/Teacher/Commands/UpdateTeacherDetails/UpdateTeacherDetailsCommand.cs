using Application.Common.Results;
using Application.Teacher.Dtos;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Teacher.Commands.UpdateTeacherDetails
{
    public sealed record UpdateTeacherDetailsCommand(
        Guid Id,
        JsonPatchDocument<UpdateTeacherDetailsDto> PatchDoc) : IRequest<OperationResult>;

}