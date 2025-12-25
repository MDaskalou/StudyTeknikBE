using Domain.Abstractions.Enum;
using Domain.Common;

namespace Domain.Models.StudySessions
{
    /// <summary>
    /// Rich Domain Model for StudySession.
    /// Encapsulates all business logic and validation for study sessions.
    /// Maps to anemic StudySessionsEntity for EF Core persistence.
    /// </summary>
    public sealed class StudySession
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public string SessionGoal { get; internal set; }
        public DateTime StartDateUtc { get; internal set; }
        public DateTime? EndDateUtc { get; internal set; }
        public int PlannedMinutes { get; internal set; }
        public int ActualMinutes { get; internal set; }
        public int EnergyStart { get; internal set; }
        public int EnergyEnd { get; internal set; }
        public StudySessionStatus Status { get; internal set; }
        public DateTime CreatedAtUtc { get; internal set; }
        public DateTime UpdatedAtUtc { get; internal set; }

        private readonly List<StudySessionStep> _steps = new();
        public IReadOnlyList<StudySessionStep> Steps => _steps.AsReadOnly();

        // EF Core constructor
        private StudySession() { }

        // Internal constructor
        private StudySession(
            Guid id,
            Guid userId,
            Guid courseId,
            string sessionGoal,
            DateTime startDateUtc,
            int plannedMinutes,
            int energyStart)
        {
            Id = id;
            UserId = userId;
            CourseId = courseId;
            SessionGoal = sessionGoal;
            StartDateUtc = startDateUtc;
            EndDateUtc = null;
            PlannedMinutes = plannedMinutes;
            ActualMinutes = 0;
            EnergyStart = energyStart;
            EnergyEnd = 0;
            Status = StudySessionStatus.Planned;
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Factory method to create a new study session
        /// </summary>
        public static Result<StudySession> Create(
            Guid userId,
            Guid courseId,
            string sessionGoal,
            int plannedMinutes,
            int energyStart)
        {
            if (userId == Guid.Empty)
                return Result<StudySession>.Failure("UserId is required.");

            if (courseId == Guid.Empty)
                return Result<StudySession>.Failure("CourseId is required.");

            if (string.IsNullOrWhiteSpace(sessionGoal))
                return Result<StudySession>.Failure("Session goal is required.");

            if (plannedMinutes <= 0 || plannedMinutes > 480)
                return Result<StudySession>.Failure("Planned minutes must be between 1 and 480.");

            if (energyStart < 1 || energyStart > 10)
                return Result<StudySession>.Failure("Energy start must be between 1 and 10.");

            var session = new StudySession(
                Guid.NewGuid(),
                userId,
                courseId,
                sessionGoal,
                DateTime.UtcNow,
                plannedMinutes,
                energyStart);

            return Result<StudySession>.Success(session);
        }

        /// <summary>
        /// Load existing session from persistence
        /// </summary>
        public static StudySession Load(
            Guid id,
            Guid userId,
            Guid courseId,
            string sessionGoal,
            DateTime startDateUtc,
            DateTime? endDateUtc,
            int plannedMinutes,
            int actualMinutes,
            int energyStart,
            int energyEnd,
            StudySessionStatus status,
            DateTime createdAtUtc,
            DateTime updatedAtUtc)
        {
            return new StudySession(id, userId, courseId, sessionGoal, startDateUtc, plannedMinutes, energyStart)
            {
                EndDateUtc = endDateUtc,
                ActualMinutes = actualMinutes,
                EnergyEnd = energyEnd,
                Status = status,
                CreatedAtUtc = createdAtUtc,
                UpdatedAtUtc = updatedAtUtc
            };
        }

        /// <summary>
        /// Load steps into session (used by repository after loading)
        /// </summary>
        public void LoadSteps(List<StudySessionStep> steps)
        {
            _steps.Clear();
            _steps.AddRange(steps);
        }

        /// <summary>
        /// Add a new step to the session
        /// </summary>
        public Result<StudySessionStep> AddStep(
            SessionStepType stepType,
            string description,
            int durationMinutes)
        {
            var stepResult = StudySessionStep.Create(
                _steps.Count,  // Use current step count as order index
                stepType,
                description,
                durationMinutes);

            if (stepResult.IsFailure)
                return stepResult;

            _steps.Add(stepResult.Value);
            UpdatedAtUtc = DateTime.UtcNow;
            return stepResult;
        }

        /// <summary>
        /// Mark step as completed
        /// </summary>
        public void CompleteStep(Guid stepId)
        {
            var step = _steps.FirstOrDefault(s => s.Id == stepId);
            if (step != null)
            {
                step.Complete();
                UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Start the session
        /// </summary>
        public void Start()
        {
            if (Status != StudySessionStatus.Planned)
                throw new InvalidOperationException("Only planned sessions can be started.");

            Status = StudySessionStatus.InProgress;
            StartDateUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// End the session
        /// </summary>
        public void End(int energyEnd)
        {
            if (Status != StudySessionStatus.InProgress)
                throw new InvalidOperationException("Only in-progress sessions can be ended.");

            if (energyEnd < 1 || energyEnd > 10)
                throw new ArgumentException("Energy end must be between 1 and 10.");

            Status = StudySessionStatus.Completed;
            EndDateUtc = DateTime.UtcNow;
            EnergyEnd = energyEnd;
            ActualMinutes = (int)(EndDateUtc.Value - StartDateUtc).TotalMinutes;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Abandon the session
        /// </summary>
        public void Abandon()
        {
            if (Status == StudySessionStatus.Completed || Status == StudySessionStatus.Abandoned)
                throw new InvalidOperationException("Cannot abandon a completed or already abandoned session.");

            Status = StudySessionStatus.Abandoned;
            EndDateUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}

