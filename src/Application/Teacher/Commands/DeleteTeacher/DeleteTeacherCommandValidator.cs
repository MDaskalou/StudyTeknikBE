using FluentValidation;

namespace Application.Teacher.Commands.DeleteTeacher
{
    public sealed class DeleteTeacherCommandValidator : AbstractValidator<DeleteTeacherCommand>
    {
        public DeleteTeacherCommandValidator()
        {
            RuleFor(t => t.Id).NotEmpty().WithMessage("Id cannot be empty.");
        }
    }
}