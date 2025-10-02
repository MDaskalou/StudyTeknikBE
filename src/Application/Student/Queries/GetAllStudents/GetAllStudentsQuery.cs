using Application.Common.Results;
using Application.Student.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.Student.Queries.GetAllStudents
{
    public sealed record GetAllStudentsQuery() 
        : IRequest<OperationResult<IReadOnlyList<StudentDetailsDto>>>;
}