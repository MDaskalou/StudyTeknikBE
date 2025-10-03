using FluentValidation;

namespace Application.Diary.Commands.UpdateDiary
{
    public class UpdateDiaryCommandValidator : AbstractValidator<UpdateDiaryCommand>
    {
        public UpdateDiaryCommandValidator()
        {
            RuleFor(d => d.Id).NotEmpty();
            RuleFor(d => d.Text)
                .NotEmpty().WithMessage("Dagboken får inte vara tom")
                .MaximumLength(5000).WithMessage("Texten får inte vara längre än 5000 tecken");
        }
    }
}