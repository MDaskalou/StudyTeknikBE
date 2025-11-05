namespace Domain.Entities
{
    public class StudySession
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SubjectId { get; set; }
        public DateTime CompletedAtUtc { get; set; }
        public string TaskDescription { get; set; }
        public int WorkDurationMinutes { get; set; }
        public string WorkFeedback { get; set; }
        public string BreakFeedback { get; set; }
        
    }
}