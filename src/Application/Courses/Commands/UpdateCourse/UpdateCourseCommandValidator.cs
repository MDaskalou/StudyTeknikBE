using FluentValidation;

namespace Application.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("CourseId is required");

            RuleFor(x => x.StudentProfileId)
                .NotEqual(Guid.Empty).WithMessage("StudentProfileId is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(100).WithMessage("Course name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Difficulty)
                .IsInEnum().WithMessage("Invalid difficulty level");
        }
    }
}

