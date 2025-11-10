namespace Domain.Entities
{
    public class StudyPlanTasksEntity
    {
        public Guid Id { get; set; }
        public Guid StudyGoalId { get; set; }
        public string Description { get; set; }
        public int SuggestedDuration { get; set; }
        public bool IsCompleted { get; set; }
    }
}