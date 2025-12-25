using Application.Common.Results;
using Application.StudySessions.DTOs;
using Application.StudySessions.Repository;
using Application.StudySessions.Mappers;
using Domain.Common;
using MediatR;

namespace Application.StudySessions.Commands.StartStudySession
{
    public sealed class StartStudySessionCommandHandler
        : IRequestHandler<StartStudySessionCommand, OperationResult<StudySessionDto>>
    {
        private readonly IStudySessionRepository _repository;

        public StartStudySessionCommandHandler(IStudySessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<StudySessionDto>> Handle(
            StartStudySessionCommand request,
            CancellationToken cancellationToken)
        {
            // STEP 1: Fetch the session
            var session = await _repository.GetByIdAsync(request.SessionId, cancellationToken);

            if (session == null)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.NotFound("StudySession.NotFound", "Study session not found."));
            }

            // STEP 2: Call business logic (Start)
            try
            {
                session.Start();
            }
            catch (InvalidOperationException ex)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.Conflict("StudySession.InvalidStatus", ex.Message));
            }

            // STEP 3: Update in repository
            await _repository.UpdateAsync(session, cancellationToken);

            // STEP 4: Return updated DTO
            var dto = session.ToDto();
            return OperationResult<StudySessionDto>.Success(dto);
        }
    }
}

