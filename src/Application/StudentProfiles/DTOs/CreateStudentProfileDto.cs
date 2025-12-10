namespace Application.StudentProfiles.DTOs
{
    public record CreateStudentProfileDto(
        int PlanningHorizonWeeks,
        TimeSpan WakeUpTime,
        TimeSpan BedTime)
    {
        
    }
}