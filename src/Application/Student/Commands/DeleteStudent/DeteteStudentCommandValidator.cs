using FluentValidation;

namespace Application.Student.Commands.DeleteStudent
{
    public class DeteteUserCommandValidator : AbstractValidator<DeleteStudentCommand>
    {
        public DeteteUserCommandValidator()
        {
            RuleFor(s => s.Id).NotEmpty();
        }
    }
}