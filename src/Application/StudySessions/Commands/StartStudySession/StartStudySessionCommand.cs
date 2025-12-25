using Application.Common.Results;
using Application.StudySessions.DTOs;
using MediatR;

namespace Application.StudySessions.Commands.StartStudySession
{
    public record StartStudySessionCommand(Guid SessionId) 
        : IRequest<OperationResult<StudySessionDto>>;
}

