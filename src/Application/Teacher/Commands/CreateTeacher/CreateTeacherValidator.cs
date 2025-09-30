using FluentValidation;

namespace Application.Teacher.Commands.CreateTeacher
{
    public class CreateTeacherValidator: AbstractValidator<CreateTeacherCommand>
    {
        public CreateTeacherValidator()
        {
            RuleFor(t => t.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(t => t.LastName).NotEmpty().MaximumLength(50);
            RuleFor(t => t.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(t => t.SecurityNumber).NotEmpty().MaximumLength(20);
            RuleFor(t => t.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}