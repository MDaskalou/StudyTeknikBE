using Application.Courses.DTOs.Course;
using Domain.Common;
using Domain.Models.StudentProfiles;

namespace Application.Courses.Mappers
{
    public static class CourseMappers
    {
        /// <summary>
        /// Map Domain Entity to DTO
        /// </summary>
        public static CourseDto ToDto(this Course domain)
        {
            return new CourseDto(
                Id: domain.Id,
                Name: domain.Name,
                Description: domain.Description,
                Difficulty: domain.Difficulty,
                CreatedAtUtc: domain.CreatedAtUtc,
                UpdatedAtUtc: domain.UpdatedAtUtc
            );
        }

    }
}

