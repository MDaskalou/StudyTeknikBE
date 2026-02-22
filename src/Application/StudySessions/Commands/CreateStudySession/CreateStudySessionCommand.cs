using Application.StudySessions.DTOs;
using Domain.Models.StudySessions;
using MediatR;
using Application.Common.Results;

namespace Application.StudySessions.Commands.CreateStudySession
{
    public sealed record CreateStudySessionCommand(
        Guid CourseId,
        string SessionGoal,
        int PlannedMinutes,
        int EnergyStart,
        // AI-kontext — sparas INTE i databasen, används bara för att generera steg
        int DifficultyLevel,
        int MotivationLevel,
        string? LearningChallenges,
        string? StudyEnvironment,
        string? AdditionalContext,
        List<CreateStudySessionStepRequest>? Steps
    ) : IRequest<OperationResult<StudySessionDto>>;
}

