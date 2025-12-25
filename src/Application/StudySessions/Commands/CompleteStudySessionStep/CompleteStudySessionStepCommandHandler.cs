using Application.Common.Results;
using Application.StudySessions.DTOs;
using Application.StudySessions.Repository;
using Application.StudySessions.Mappers;
using Domain.Common;
using MediatR;

namespace Application.StudySessions.Commands.CompleteStudySessionStep
{
    public sealed class CompleteStudySessionStepCommandHandler
        : IRequestHandler<CompleteStudySessionStepCommand, OperationResult<StudySessionDto>>
    {
        private readonly IStudySessionRepository _repository;

        public CompleteStudySessionStepCommandHandler(IStudySessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<StudySessionDto>> Handle(
            CompleteStudySessionStepCommand request,
            CancellationToken cancellationToken)
        {
            // STEP 1: Fetch the session
            var session = await _repository.GetByIdAsync(request.SessionId, cancellationToken);

            if (session == null)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.NotFound("StudySession.NotFound", "Study session not found."));
            }

            // STEP 2: Verify step exists in session
            var stepExists = session.Steps.Any(s => s.Id == request.StepId);
            if (!stepExists)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.NotFound("StudySessionStep.NotFound", "Step not found in this session."));
            }

            // STEP 3: Call business logic (CompleteStep)
            session.CompleteStep(request.StepId);

            // STEP 4: Update in repository
            await _repository.UpdateAsync(session, cancellationToken);

            // STEP 5: Return updated DTO
            var dto = session.ToDto();
            return OperationResult<StudySessionDto>.Success(dto);
        }
    }
}

