using FluentValidation;

namespace Application.FlashCards.Commands.DeleteFlashCard
{
    public class DeleteFlashCardCommandValidator : AbstractValidator<DeleteFlashCardCommand>
    {
        public DeleteFlashCardCommandValidator()
        {
            RuleFor(x => x.FlashCardId)
                .NotEqual(Guid.Empty).WithMessage("FlashCard ID kan inte vara tom");

            RuleFor(x => x.DeckId)
                .NotEqual(Guid.Empty).WithMessage("Deck ID kan inte vara tom");
        }
    }
}

