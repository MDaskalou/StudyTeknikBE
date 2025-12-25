using Domain.Abstractions.Enum;

namespace Application.StudySessions.DTOs
{
    public record StudySessionDto(
        Guid Id,
        Guid UserId,
        Guid CourseId,
        string SessionGoal,
        DateTime StartDateUtc,
        DateTime? EndDateUtc,
        int PlannedMinutes,
        int ActualMinutes,
        int EnergyStart,
        int EnergyEnd,
        StudySessionStatus Status,
        List<StudySessionStepDto> Steps,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}

