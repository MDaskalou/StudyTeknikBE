using FluentValidation;

namespace Application.FlashCards.Commands.UpdateFlashCard
{
    public class UpdateFlashCardCommandValidator : AbstractValidator<UpdateFlashCardCommand>
    {
        public UpdateFlashCardCommandValidator()
        {
            RuleFor(x => x.FlashCardId)
                .NotEqual(Guid.Empty).WithMessage("FlashCard ID kan inte vara tom");

            RuleFor(x => x.DeckId)
                .NotEqual(Guid.Empty).WithMessage("Deck ID kan inte vara tom");

            RuleFor(x => x.FrontText)
                .NotEmpty().WithMessage("FrontText kan inte vara tom")
                .MaximumLength(500).WithMessage("FrontText får max vara 500 tecken");

            RuleFor(x => x.BackText)
                .NotEmpty().WithMessage("BackText kan inte vara tom")
                .MaximumLength(4000).WithMessage("BackText får max vara 4000 tecken");
        }
    }
}

