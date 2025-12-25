using Application.StudySessions.DTOs;
using Application.Common.Results;
using MediatR;

namespace Application.StudySessions.Queries.GetStudySessionById
{
    public record GetStudySessionByIdQuery(Guid SessionId) : IRequest<OperationResult<StudySessionDto>>;
}

