using Application.Common.Results;
using Application.StudySessions.DTOs;
using MediatR;

namespace Application.StudySessions.Commands.CompleteStudySessionStep
{
    public record CompleteStudySessionStepCommand(
        Guid SessionId,
        Guid StepId) 
        : IRequest<OperationResult<StudySessionDto>>;
}

