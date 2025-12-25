using Domain.Abstractions.Enum;

namespace Application.StudySessions.DTOs
{
    public record StudySessionStepDto(
        Guid Id,
        int OrderIndex,
        SessionStepType StepType,
        string Description,
        int DurationMinutes,
        bool IsCompleted,
        DateTime CreatedAtUtc
    );
}

