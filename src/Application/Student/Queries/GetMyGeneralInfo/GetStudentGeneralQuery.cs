using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;

namespace Application.Student.Queries.GetMyGeneralInfo
{
    public record GetStudentGeneralInfoQuery : IRequest<OperationResult<StudentGeneralDto>>;
}