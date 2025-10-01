using Application.Common.Results;
using MediatR;

namespace Application.Teacher.Commands.DeleteTeacher
{
    public sealed record DeleteTeacherCommand(Guid Id) : IRequest<OperationResult>;
    
        
    
}