using FluentValidation;
using Domain.Abstractions.Enum;

namespace Application.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.StudentProfileId)
                .NotEqual(Guid.Empty).WithMessage("StudentProfileId is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(100).WithMessage("Course name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Difficulty)
                .IsInEnum().WithMessage("Invalid difficulty level. Must be 0 (Easy), 1 (Medium), or 2 (Hard)");
        }
    }
}

