using Application.Common.Results;
using Application.Courses.DTOs.Course;
using MediatR;

namespace Application.Courses.Queries.GetCourseById
{
    public sealed record GetCourseByIdQuery(
        Guid CourseId,
        Guid StudentProfileId
    ) : IRequest<OperationResult<CourseDto>>;
}

