namespace Domain.Entities
{
    public class StudySessionsEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SubjectId { get; set; }
        public DateTime StartDate { get; set; }
        public string TaskDescription { get; set; }
        public int WorkDurationMinutes { get; set; }
        public string WorkFeedback { get; set; }
        public string BreakFeedback { get; set; }
        
    }
}   