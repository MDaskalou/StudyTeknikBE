namespace Domain.Entities
{
    public sealed class StudentProfileEntity
    {
        public Guid Id { get; set; }
        
        public Guid StudentId { get; set; }
        public UserEntity? User { get; set; } 

        // Data för AI-planering
        public int PlanningHorizonWeeks { get; set; }
        public TimeSpan WakeUpTime { get; set; }
        public TimeSpan BedTime { get; set; }

        // Navigation property: En profil har många kurser
        public ICollection<CourseEntity> Courses { get; set; } = new List<CourseEntity>();

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}