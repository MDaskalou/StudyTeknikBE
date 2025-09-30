using Application.Common.Results;
using Application.Teacher.Dtos;
using MediatR;

namespace Application.Teacher.Commands.CreateTeacher
{
    public sealed record CreateTeacherCommand(
        string FirstName,
        string LastName,
        string Email,
        string SecurityNumber
    ) : IRequest<OperationResult<CreateTeacherDto>>;
}