using Application.Common.Results;
using MediatR;

namespace Application.Student.Commands.DeleteStudent
{
    public record DeleteStudentCommand(Guid Id) : IRequest<OperationResult>;



}