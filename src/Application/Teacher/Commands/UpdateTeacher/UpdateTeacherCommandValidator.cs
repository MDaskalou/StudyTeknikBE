using FluentValidation;

namespace Application.Teacher.Commands.UpdateTeacher
{
    public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
    {
        public UpdateTeacherCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty();
            RuleFor(t => t.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(t => t.LastName).NotEmpty().MaximumLength(100);
            RuleFor(t => t.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(t=> t.SecurityNumber).NotEmpty().Matches(@"^\d{6}[-+]?\d{4}$").WithMessage("Security number must be in the format YYMMDD-XXXX or YYMMDD+XXXX");
        }
        
    }
}