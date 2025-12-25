using Domain.Abstractions.Enum;

namespace Domain.Entities
{
    /// <summary>
    /// Anemic Entity for EF Core Persistence - Individual study session steps.
    /// Maps to rich domain model StudySessionStep for business logic.
    /// </summary>
    public class StudySessionStepEntity
    {
        public Guid Id { get; set; }
        public Guid StudySessionId { get; set; }
        
        public int OrderIndex { get; set; }
        public SessionStepType StepType { get; set; }
        public string Description { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        
        // Navigation
        public StudySessionsEntity? StudySession { get; set; }
    }
}

