using FluentValidation;

namespace Application.Diary.Commands.CreateDiary
{
    public class CreateDiaryCommandValidator : AbstractValidator<CreateDiaryCommand>
    {
        public CreateDiaryCommandValidator()
        {
            RuleFor(d => d.EntryDate)
                .NotEmpty().WithMessage("Datum måste anges");

            RuleFor(d => d.Text)
                .NotEmpty().WithMessage("Studiedagboken får inte vara tom.")
                .MaximumLength(5000).WithMessage("Texten får inte vara längre än 5000 tecken.");

        }
    }
}