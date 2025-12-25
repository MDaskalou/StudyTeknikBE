using Domain.Abstractions.Enum;
using Domain.Common;

namespace Domain.Models.StudySessions
{
    /// <summary>
    /// Rich Domain Model for StudySessionStep.
    /// Encapsulates business logic for individual study session steps.
    /// </summary>
    public sealed class StudySessionStep
    {
        public Guid Id { get; private set; }
        public int OrderIndex { get; private set; }
        public SessionStepType StepType { get; internal set; }
        public string Description { get; internal set; }
        public int DurationMinutes { get; internal set; }
        public bool IsCompleted { get; internal set; }
        public DateTime CreatedAtUtc { get; internal set; }

        // EF Core constructor
        private StudySessionStep() { }

        // Internal constructor
        private StudySessionStep(
            Guid id,
            int orderIndex,
            SessionStepType stepType,
            string description,
            int durationMinutes)
        {
            Id = id;
            OrderIndex = orderIndex;
            StepType = stepType;
            Description = description;
            DurationMinutes = durationMinutes;
            IsCompleted = false;
            CreatedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Factory method to create a new study session step
        /// </summary>
        public static Result<StudySessionStep> Create(
            int orderIndex,
            SessionStepType stepType,
            string description,
            int durationMinutes)
        {
            if (orderIndex < 0)
                return Result<StudySessionStep>.Failure("Order index cannot be negative.");

            if (string.IsNullOrWhiteSpace(description))
                return Result<StudySessionStep>.Failure("Step description is required.");

            if (durationMinutes <= 0)
                return Result<StudySessionStep>.Failure("Step duration must be greater than 0.");

            var step = new StudySessionStep(
                Guid.NewGuid(),
                orderIndex,
                stepType,
                description,
                durationMinutes);

            return Result<StudySessionStep>.Success(step);
        }

        /// <summary>
        /// Load existing step from persistence
        /// </summary>
        public static StudySessionStep Load(
            Guid id,
            int orderIndex,
            SessionStepType stepType,
            string description,
            int durationMinutes,
            bool isCompleted,
            DateTime createdAtUtc)
        {
            return new StudySessionStep(id, orderIndex, stepType, description, durationMinutes)
            {
                IsCompleted = isCompleted,
                CreatedAtUtc = createdAtUtc
            };
        }

        /// <summary>
        /// Mark step as completed
        /// </summary>
        public void Complete()
        {
            IsCompleted = true;
        }
    }
}

