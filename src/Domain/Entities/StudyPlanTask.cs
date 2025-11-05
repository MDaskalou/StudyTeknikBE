namespace Domain.Entities
{
    public class StudyPlanTask
    {
       public Guid Id { get; set; }
       public Guid StudyGoalId { get; set; }
       public string TaskDescription { get; set; }
       public int SuggestedDuration { get; set; } 
       public bool IsCompleted { get; set; }
    }
}