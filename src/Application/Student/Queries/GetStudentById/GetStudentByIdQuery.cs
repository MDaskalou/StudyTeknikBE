using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;

namespace Application.Student.Queries.GetStudentById

{
    public sealed record GetStudentByIdQuery(Guid Id) : IRequest<OperationResult<GetStudentByIdDto>>;
}