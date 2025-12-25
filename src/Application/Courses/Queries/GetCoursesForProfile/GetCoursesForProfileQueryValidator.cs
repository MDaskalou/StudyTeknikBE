using FluentValidation;

namespace Application.Courses.Queries.GetCoursesForProfile
{
    public class GetCoursesForProfileQueryValidator : AbstractValidator<GetCoursesForProfileQuery>
    {
        public GetCoursesForProfileQueryValidator()
        {
            RuleFor(x => x.StudentProfileId)
                .NotEqual(Guid.Empty).WithMessage("StudentProfileId is required");
        }
    }
}

