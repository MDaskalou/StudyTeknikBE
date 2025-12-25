using Application.Common.Results;
using Application.Courses.DTOs.Course;
using MediatR;

namespace Application.Courses.Queries.GetCoursesForProfile
{
    public sealed record GetCoursesForProfileQuery(
        Guid StudentProfileId
    ) : IRequest<OperationResult<List<CourseDto>>>;
}

