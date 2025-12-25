using Domain.Abstractions.Enum;

namespace Application.Courses.DTOs.Course
{
    public record CourseDto(
        Guid Id,
        string Name,
        string? Description,
        CourseDifficulty Difficulty,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}

