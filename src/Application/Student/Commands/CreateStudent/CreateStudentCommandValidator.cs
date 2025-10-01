using FluentValidation;

namespace Application.Student.Commands.CreateStudent
{
    //todo: Skapa student (User med Role.Student). Koppla ev. till Class.
    public sealed class CreateStudentCommandValidator: AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(s => s.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            
            RuleFor(s => s.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
            RuleFor(s=>s.ClassId)
                .NotEmpty().WithMessage("ClassId is required.");
            RuleFor(s => s.SecurityNumber)
                .NotEmpty().WithMessage("Personnummer får inte vara tom.");
        }
    }
}