using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;

namespace Application.Student.Queries.GetAllStudents

{
    public sealed record GetAllStudentsQuery() : IRequest<IReadOnlyList<StudentDetailsDto>>;
}