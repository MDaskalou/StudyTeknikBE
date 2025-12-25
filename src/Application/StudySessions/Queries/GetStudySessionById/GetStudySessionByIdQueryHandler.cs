using Application.StudySessions.DTOs;
using Application.Common.Results;
using Application.StudySessions.Repository;
using Application.StudySessions.Mappers;
using Domain.Common;
using MediatR;

namespace Application.StudySessions.Queries.GetStudySessionById
{
    public sealed class GetStudySessionByIdQueryHandler
        : IRequestHandler<GetStudySessionByIdQuery, OperationResult<StudySessionDto>>
    {
        private readonly IStudySessionRepository _repository;

        public GetStudySessionByIdQueryHandler(IStudySessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<StudySessionDto>> Handle(
            GetStudySessionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var session = await _repository.GetByIdAsync(request.SessionId, cancellationToken);

            if (session == null)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.NotFound("StudySession.NotFound", "Study session not found."));
            }

            var dto = session.ToDto();
            return OperationResult<StudySessionDto>.Success(dto);
        }
    }
}

