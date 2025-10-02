using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;
using FluentValidation;

namespace Application.Student.Commands.CreateStudent
{
    
    // TODO: Skapa student (User med Role.Student). Koppla ev. till Class.

    public sealed record CreateStudentCommand(string FirstName, string LastName, string Email,string SecurityNumber ,Guid ClassId)
        : IRequest<OperationResult<StudentCreatedDto>>;

}