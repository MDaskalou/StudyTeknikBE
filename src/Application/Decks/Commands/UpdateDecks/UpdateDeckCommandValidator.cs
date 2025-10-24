using Application.Decks.IRepository;
using FluentValidation;

namespace Application.Decks.Commands.UpdateDecks
{
    public sealed class UpdateDeckCommandValidator : AbstractValidator<UpdateDeckCommand>
    {
        private readonly IDeckRepository _deckRepository;

        public UpdateDeckCommandValidator(IDeckRepository deckRepository)
        {
            RuleFor(d => d.Title)
                .NotEmpty().WithMessage("Titeln får inte vara tom.")
                .MaximumLength(50).WithMessage("Titeln får inte överstiga 50 tecken.");
            
            RuleFor(d => d.CourseName)
                .NotEmpty().WithMessage("Kurnnamnet får inte vara tomt.")
                .MaximumLength(50).WithMessage("Kursnamnet får inte överstiga 50 tecken.");
            
            RuleFor(d => d.SubjectName)
                .NotEmpty().WithMessage("Ämnesnamnet får inte vara tomt.")
                .MaximumLength(50).WithMessage("Ämnesnamnet får inte överstiga 50 tecken.");
        }
    }
}