using Application.Common.Results;
using MediatR;

namespace Application.Teacher.Commands.UpdateTeacher
{
    public sealed record UpdateTeacherCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string SecurityNumber
    ) : IRequest<OperationResult>;
    
}