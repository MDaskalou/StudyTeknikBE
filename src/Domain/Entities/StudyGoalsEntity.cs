namespace Domain.Entities
{
    public class StudyGoalsEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public Guid CourseId { get; set; }
        public CourseEntity Course { get; set; }
        
        public string GoalDescription { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        
    }
}