using Application.Common.Results;
using Application.StudySessions.DTOs;
using MediatR;

namespace Application.StudySessions.Commands.EndStudySession
{
    public record EndStudySessionCommand(
        Guid SessionId,
        int EnergyLevel) 
        : IRequest<OperationResult<StudySessionDto>>;
}

