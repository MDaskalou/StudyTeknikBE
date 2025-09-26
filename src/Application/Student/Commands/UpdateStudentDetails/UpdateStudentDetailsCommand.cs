using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace Application.Student.Commands
{
    public sealed record UpdateStudentDetailsCommand(
        Guid Id,
        JsonPatchDocument<UpdateStudentDetailsDto> PatchDoc) : IRequest<OperationResult>;
}