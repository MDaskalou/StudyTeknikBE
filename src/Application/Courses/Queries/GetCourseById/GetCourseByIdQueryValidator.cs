using FluentValidation;

namespace Application.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
    {
        public GetCourseByIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("CourseId is required");

            RuleFor(x => x.StudentProfileId)
                .NotEqual(Guid.Empty).WithMessage("StudentProfileId is required");
        }
    }
}

