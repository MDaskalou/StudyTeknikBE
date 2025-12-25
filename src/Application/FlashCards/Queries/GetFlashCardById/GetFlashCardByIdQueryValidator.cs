using FluentValidation;

namespace Application.FlashCards.Queries.GetFlashCardById
{
    public class GetFlashCardByIdQueryValidator : AbstractValidator<GetFlashCardByIdQuery>
    {
        public GetFlashCardByIdQueryValidator()
        {
            RuleFor(x => x.FlashCardId)
                .NotEqual(Guid.Empty).WithMessage("FlashCard ID kan inte vara tom");

            RuleFor(x => x.DeckId)
                .NotEqual(Guid.Empty).WithMessage("Deck ID kan inte vara tom");
        }
    }
}

