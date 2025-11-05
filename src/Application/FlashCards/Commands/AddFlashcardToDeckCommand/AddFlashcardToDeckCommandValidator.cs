using FluentValidation;

namespace Application.FlashCards.Commands.AddFlashcardToDeckCommand
{
    public class AddFlashcardToDeckCommandValidator: AbstractValidator<AddFlashcardToDeckCommand.AddFlashcardToDeckCommand>
    {
        public AddFlashcardToDeckCommandValidator()
        {
            RuleFor(d => d.FrontText)
                .NotEmpty().WithMessage("Texten får inte vara tom.")
                .MaximumLength(250).WithMessage("Det får vara max 250 tecken.");
            
            RuleFor(d => d.BackText)
                .NotEmpty().WithMessage("Texten får inte vara tom.")
                .MaximumLength(1000).WithMessage("Det får vara max 1000 tecken.");
        }
    }
}