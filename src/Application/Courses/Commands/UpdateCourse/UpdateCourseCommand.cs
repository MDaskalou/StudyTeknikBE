using Application.Common.Results;
using Domain.Abstractions.Enum;
using MediatR;

namespace Application.Courses.Commands.UpdateCourse
{
    public sealed record UpdateCourseCommand(
        Guid CourseId,
        Guid StudentProfileId,
        string Name,
        string? Description,
        CourseDifficulty Difficulty
    ) : IRequest<OperationResult<Application.Courses.DTOs.Course.CourseDto>>;
}

