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
        List<CreateStudySessionStepRequest> Steps
    ) : IRequest<OperationResult<StudySessionDto>>;
}

