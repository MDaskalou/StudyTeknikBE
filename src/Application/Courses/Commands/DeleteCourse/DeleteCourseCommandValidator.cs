using FluentValidation;

namespace Application.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("CourseId is required");

            RuleFor(x => x.StudentProfileId)
                .NotEqual(Guid.Empty).WithMessage("StudentProfileId is required");
        }
    }
}

