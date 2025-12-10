namespace Application.StudentProfiles.DTOs
{
    public record StudentProfileDto(
        Guid Id,
        Guid StudentId,
        int PlanningHorizonWeeks,
        TimeSpan WakeUpTime,
        TimeSpan BedTime
    );
}