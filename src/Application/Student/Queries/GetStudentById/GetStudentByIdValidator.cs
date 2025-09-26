using Application.Common.Results;
using FluentValidation;

namespace Application.Student.Queries.GetStudentById
{
    public sealed class GetStudentByIdValidator : AbstractValidator<GetStudentByIdQuery>
        {
            public GetStudentByIdValidator() => RuleFor(s => s.Id).NotEmpty();

        }
}