using Application.Common.Results;
using MediatR;

namespace Application.Student.Commands.UpdateStudent
{
    public sealed record UpdateStudentCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string SecurityNumber,
        Guid? ClassId) : IRequest<OperationResult>;
    
        
    
}