using Domain.Abstractions.Enum;

namespace Application.StudySessions.DTOs
{
    public record CreateStudySessionStepRequest(
        SessionStepType StepType,
        string Description,
        int DurationMinutes
    );

    public record CreateStudySessionRequest(
        Guid CourseId,
        string SessionGoal,
        int PlannedMinutes,
        int EnergyStart,
        int DifficultyLevel,
        int MotivationLevel,
        string? LearningChallenges,
        string? StudyEnvironment,
        string? AdditionalContext,
        List<CreateStudySessionStepRequest>? Steps
    );
}

