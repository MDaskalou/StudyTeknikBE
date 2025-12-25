using Application.Common.Results;
using Application.StudySessions.Commands.CreateStudySession;
using Application.StudySessions.DTOs;
using Application.StudySessions.Repository;
using Application.StudySessions.Mappers;
using Application.Abstractions.IPersistence.Repositories;
using Domain.Common;
using Domain.Models.StudySessions;
using FluentValidation;
using MediatR;

namespace Application.StudySessions.Commands.CreateStudySession
{
    public sealed class CreateStudySessionCommandHandler
        : IRequestHandler<CreateStudySessionCommand, OperationResult<StudySessionDto>>
    {
        private readonly IStudySessionRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateStudySessionCommand> _validator;

        public CreateStudySessionCommandHandler(
            IStudySessionRepository repository,
            ICurrentUserService currentUserService,
            IValidator<CreateStudySessionCommand> validator)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<OperationResult<StudySessionDto>> Handle(
            CreateStudySessionCommand request,
            CancellationToken cancellationToken)
        {
            // STEP 1: Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<StudySessionDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEP 2: Get current user
            var userId = _currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "User context is required."));
            }

            // STEP 3: Create domain model
            var sessionResult = StudySession.Create(
                userId.Value,
                request.CourseId,
                request.SessionGoal,
                request.PlannedMinutes,
                request.EnergyStart);

            if (sessionResult.IsFailure)
            {
                return OperationResult<StudySessionDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, sessionResult.Error));
            }

            var session = sessionResult.Value;

            // STEP 4: Add steps to session
            foreach (var stepRequest in request.Steps)
            {
                var addStepResult = session.AddStep(
                    stepRequest.StepType,
                    stepRequest.Description,
                    stepRequest.DurationMinutes);

                if (addStepResult.IsFailure)
                {
                    return OperationResult<StudySessionDto>.Failure(
                        Error.Validation(ErrorCodes.General.Validation, addStepResult.Error));
                }
            }

            // STEP 5: Save to repository
            await _repository.AddAsync(session, cancellationToken);

            // STEP 6: Map and return
            var dto = session.ToDto();
            return OperationResult<StudySessionDto>.Success(dto);
        }
    }
}

