using Domain.Abstractions.Enum;

namespace Application.Courses.DTOs.Course
{
    public record CreateCourseRequest(
        string Name,
        string? Description,
        CourseDifficulty Difficulty
    );

    public record UpdateCourseRequest(
        string Name,
        string? Description,
        CourseDifficulty Difficulty
    );
}

