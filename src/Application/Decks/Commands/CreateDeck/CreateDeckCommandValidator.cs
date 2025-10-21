using FluentValidation;
using FluentValidation.Validators;

namespace Application.Decks.Commands.CreateDeck
{
    public class CreateDeckCommandValidator :AbstractValidator<CreateDeckCommand>
    {
        public CreateDeckCommandValidator()
        {
            RuleFor(d => d.Title)
                .NotEmpty().WithMessage("Titel kan inte vara tom")
                .MaximumLength(100).WithMessage("Titeln får inte ha mer än 100 tecken");

            RuleFor(d => d.CourseName)
                .NotEmpty().WithMessage("Kurnnamnet får ej vara tom")
                .MaximumLength(100).WithMessage("Kursnamnet har en gränns på 100tecken");

            RuleFor(d => d.SubjectName)
                .NotEmpty().WithMessage("Ämnesområde får inte vara tom")
                .MaximumLength(100).WithMessage("Ämnesområdet har är maxgräns på 100 tecken");


        }
    }
}