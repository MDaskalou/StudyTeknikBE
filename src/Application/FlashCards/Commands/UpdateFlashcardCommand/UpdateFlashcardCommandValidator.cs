using FluentValidation;

namespace Application.FlashCards.Commands.UpdateFlashcardCommand
{
    // Validatorn ärver från AbstractValidator och pekar på ditt Command
    public class UpdateFlashcardCommandValidator : AbstractValidator<UpdateFlashcardCommand>
    {
        public UpdateFlashcardCommandValidator()
        {
            RuleFor(x => x.DeckId)
                .NotEmpty().WithMessage("DeckId måste anges.");
            
            RuleFor(x => x.FlashCardId)
                .NotEmpty().WithMessage("FlashCardId måste anges.");

            // Se till att textfälten inte är tomma
            RuleFor(x => x.FrontText)
                .NotEmpty().WithMessage("Framsidan (Fråga) får inte vara tom.")
                .MaximumLength(250).WithMessage("Framsidan får max vara 250 tecken."); // Bra att ha!

            RuleFor(x => x.BackText)
                .NotEmpty().WithMessage("Baksidan (Svar) får inte vara tom.")
                .MaximumLength(1000).WithMessage("Baksidan får max vara 1000 tecken."); // Bra att ha!
        }
    }
}