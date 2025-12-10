using Domain.Abstractions.Enum;

namespace Domain.Entities
{
    public sealed class CourseEntity
    {
        public Guid Id { get; set; }
        public Guid StudentProfileId { get; set; }
        public StudentProfileEntity? StudentProfile { get; set; }

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public CourseDifficulty Difficulty { get; set; } 

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}