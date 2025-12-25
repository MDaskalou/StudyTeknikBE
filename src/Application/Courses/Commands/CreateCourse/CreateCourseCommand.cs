using Application.Common.Results;
using Application.Courses.DTOs.Course;
using Domain.Abstractions.Enum;
using MediatR;

namespace Application.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseCommand(
        Guid StudentProfileId,
        string Name,
        string? Description,
        CourseDifficulty Difficulty
    ) : IRequest<OperationResult<CourseDto>>;
}

