using Application.Common.Results;
using Application.Teacher.Dtos;
using MediatR;

namespace Application.Teacher.Queries.GetTeacherById
{
    public sealed record GetTeacherByIdQuery(Guid Id)  : IRequest<OperationResult<GetTeacherByIdDto?>>;
}