using Domain.Abstractions.Enum;

namespace Domain.Entities
{
    /// <summary>
    /// Anemic Entity for EF Core Persistence - Study Sessions with Steps.
    /// Maps to rich domain model StudySession for business logic.
    /// </summary>
    public class StudySessionsEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        
        public Guid CourseId { get; set; }
        public CourseEntity? Course { get; set; }
        
        public string SessionGoal { get; set; } = default!;
        public DateTime StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        
        public int PlannedMinutes { get; set; }
        public int ActualMinutes { get; set; }
        
        public int EnergyStart { get; set; } // 1-10 scale
        public int EnergyEnd { get; set; }   // 1-10 scale
        
        public StudySessionStatus Status { get; set; }
        
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        // Navigation - Study session steps
        public ICollection<StudySessionStepEntity> Steps { get; set; } = new List<StudySessionStepEntity>();
    }
}   

