using Application.Diary.Dtos;
using FluentValidation;

namespace Application.Diary.Commands.UpdateDiaryDetails
{
    public sealed class UpdateDiaryDetailsCommandValidator : AbstractValidator<UpdateDiaryDetailsDto>
    {
        public UpdateDiaryDetailsCommandValidator()
        {
            RuleFor(d =>d.Text)
                .NotEmpty().WithMessage("Dagboken kan inte vara tom")
                .MaximumLength(5000).WithMessage("Max antalet för  dagboken är 5000 tecken");
        }
    }
}