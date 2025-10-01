using Application.Teacher.Dtos;
using FluentValidation;

namespace Application.Teacher.Commands.UpdateTeacherDetails
{
    public class UpdateTeacherDetailsCommandValidator : AbstractValidator<UpdateTeacherDetailsDto>
    {
        public UpdateTeacherDetailsCommandValidator()
        {
            RuleFor(t => t.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(t => t.LastName).NotEmpty().MaximumLength(50);
            RuleFor(t => t.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(t => t.SecurityNumber).NotEmpty().MaximumLength(20);
            
        }
    }
}