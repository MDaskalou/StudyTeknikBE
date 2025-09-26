using FluentValidation;

namespace Application.Student.Commands.UpdateStudent
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(x => x.SecurityNumber).NotEmpty().Matches(@"^\d{6}[-+]?\d{4}$").WithMessage("Security number must be in the format YYMMDD-XXXX or YYMMDD+XXXX");
            RuleFor(x => x.ClassId).NotNull().When(x => x.ClassId.HasValue);
        }
    }
}