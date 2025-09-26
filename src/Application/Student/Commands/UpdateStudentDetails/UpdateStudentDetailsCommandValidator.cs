using Application.Student.Dtos;
using FluentValidation;

namespace Application.Student.Commands.UpdateStudentDetails
{
    public class UpdateStudentDetailsDtoValidator : AbstractValidator<UpdateStudentDetailsDto>
    {
        public UpdateStudentDetailsDtoValidator()
        {
            RuleFor(s => s.FirstName).MaximumLength(50);
            RuleFor(s => s.LastName).MaximumLength(50);
            RuleFor(s => s.Email).EmailAddress();
            RuleFor(s => s.SecurityNumber).MaximumLength(50);
        }
    }
}