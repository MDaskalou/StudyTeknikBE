using Application.Common.Results;
using Application.StudentProfiles.Commands.CreateStudentProfile;
using FluentValidation;
using FluentValidation.Validators;

namespace Application.StudentProfile.Commands.CreateStudentProfile
{
    public class CreateStudentProfileValidator : AbstractValidator<CreateStudentProfileCommand>
    {
        public CreateStudentProfileValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");

            RuleFor(x => x.PlanningHorizonWeeks)
                .GreaterThan(0).WithMessage("PlaningHorizonWeeks måste vara minst 1 vecka.")
                .LessThanOrEqualTo(52).WithMessage("PlaningHorizonWeeks kan inte överstiga 52 veckor.");
            

            RuleFor(x => x.WakeUpTime)
                .NotNull().WithMessage("WakeUpTime is required.");
            
            RuleFor(x => x.BedTime)
                .NotNull().WithMessage("BedTime is required.");
        }
    }
}