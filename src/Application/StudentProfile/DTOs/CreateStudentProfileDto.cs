namespace Application.StudentProfile.DTOs
{
    public record CreateStudentProfileDto(
        int PlanningHorizonWeeks,
        TimeSpan WakeUpTime,
        TimeSpan BedTime)
    {
        
    }
}