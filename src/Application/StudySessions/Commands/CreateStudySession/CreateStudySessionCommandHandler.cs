using Application.Abstractions.IPersistence;
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
        private readonly IAIService _aiService;

        public CreateStudySessionCommandHandler(
            IStudySessionRepository repository,
            ICurrentUserService currentUserService,
            IValidator<CreateStudySessionCommand> validator,
            IAIService aiService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _validator = validator;
            _aiService = aiService;
        }

        public async Task<OperationResult<StudySessionDto>> Handle(
    CreateStudySessionCommand request,
    CancellationToken cancellationToken)
{
    // STEP 1: Validering (FluentValidation körs här)
    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    if (!validationResult.IsValid)
    {
        var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
        return OperationResult<StudySessionDto>.Failure(
            Error.Validation(ErrorCodes.General.Validation, errorMessages));
    }

    // STEP 2: Hämta användare
    var userId = _currentUserService.UserId;
    if (userId == null || userId == Guid.Empty)
    {
        return OperationResult<StudySessionDto>.Failure(
            Error.Forbidden(ErrorCodes.General.Forbidden, "User context is required."));
    }

    // STEP 3: Skapa domänmodellen (Sessionen)
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

    // STEP 4: Hantera steg (Logik för att välja mellan Manuella steg eller AI-genererade)
    // Vi kollar om användaren skickat med egna steg, annars ropar vi på AI-servern
    var finalSteps = (request.Steps != null && request.Steps.Any())
        ? request.Steps
        : await _aiService.GenerateStudyStepsAsync(
            request.SessionGoal,
            request.PlannedMinutes,
            request.EnergyStart,
            request.DifficultyLevel,
            request.MotivationLevel,
            request.LearningChallenges,
            request.StudyEnvironment,
            request.AdditionalContext,
            cancellationToken);

    // Lägg till de valda stegen i vår domänmodell
    foreach (var step in finalSteps)
    {
        var addStepResult = session.AddStep(
            step.StepType,
            step.Description,
            step.DurationMinutes);

        if (addStepResult.IsFailure)
        {
            return OperationResult<StudySessionDto>.Failure(
                Error.Validation(ErrorCodes.General.Validation, addStepResult.Error));
        }
    }

    // STEP 5: Spara sessionen (inklusive alla nya steg) i databasen
    await _repository.AddAsync(session, cancellationToken);

    // STEP 6: Mappa till DTO och returnera till frontend
    var dto = session.ToDto();
    return OperationResult<StudySessionDto>.Success(dto);
}
    }
}

