using FluentValidation;

namespace Application.FlashCards.Commands
{
    public class AddFlashcardToDeckCommandValidator: AbstractValidator<AddFlashcardToDeckCommand>
    {
        public AddFlashcardToDeckCommandValidator()
        {
            RuleFor(d => d.FrontText)
                .NotEmpty().WithMessage("Texten får inte vara tom.")
                .MaximumLength(50).WithMessage("Det får vara max 50 tecken.");
            
            RuleFor(d => d.BackText)
                .NotEmpty().WithMessage("Texten får inte vara tom.")
                .MaximumLength(500).WithMessage("Det får vara max 500 tecken.");
        }
    }
}