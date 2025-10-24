using FluentValidation;

namespace Application.Decks.Commands.DeleteDecks
{
    public class DeleteDecksCommandValidator: AbstractValidator<DeleteDecksCommand>
    {
        public DeleteDecksCommandValidator()
        {
            RuleFor(d => d.DeckId)
                .NotEmpty().WithMessage("Id får inte vara tomt.");
        }
    }

    
}