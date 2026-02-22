using Application.StudySessions.Commands.CreateStudySession;
using FluentValidation;

namespace Application.StudySessions.Commands.CreateStudySession
{
    public sealed class CreateStudySessionCommandValidator : AbstractValidator<CreateStudySessionCommand>
    {
        public CreateStudySessionCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");

            RuleFor(x => x.SessionGoal)
                .NotEmpty().WithMessage("Session goal is required.")
                .MaximumLength(500).WithMessage("Session goal cannot exceed 500 characters.");

            RuleFor(x => x.PlannedMinutes)
                .GreaterThan(0).WithMessage("Planned minutes must be greater than 0.")
                .LessThanOrEqualTo(480).WithMessage("Planned minutes cannot exceed 8 hours (480 minutes).");

            RuleFor(x => x.EnergyStart)
                .InclusiveBetween(1, 10).WithMessage("Energy start level must be between 1 and 10.");

            
            When(x => x.Steps != null && x.Steps.Any(), () =>
            {
                RuleFor(x => x.Steps)
                    .Must(s => s!.Count <= 20).WithMessage("Maximum 20 steps allowed per session.");

                RuleForEach(x => x.Steps).ChildRules(step =>
                {
                    step.RuleFor(s => s.Description)
                        .NotEmpty().WithMessage("Step description is required.")
                        .MaximumLength(300).WithMessage("Step description cannot exceed 300 characters.");

                    step.RuleFor(s => s.DurationMinutes)
                        .GreaterThan(0).WithMessage("Step duration must be greater than 0.")
                        .LessThanOrEqualTo(120).WithMessage("Step duration cannot exceed 2 hours.");
                });
            });
        }
    }
}

