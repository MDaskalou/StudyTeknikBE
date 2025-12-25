using Application.Common.Results;
using Application.Decks.IRepository;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.FlashCards.Commands.DeleteFlashCard
{
    public sealed class DeleteFlashCardCommandHandler 
        : IRequestHandler<DeleteFlashCardCommand, OperationResult>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly IValidator<DeleteFlashCardCommand> _validator;

        public DeleteFlashCardCommandHandler(
            IDeckRepository deckRepository,
            IValidator<DeleteFlashCardCommand> validator)
        {
            _deckRepository = deckRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(
            DeleteFlashCardCommand request, CancellationToken cancellationToken)
        {
            // STEG 1: Validera
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEG 2: Hämta decket med tracked
            var deck = await _deckRepository.GetByIdTrackedAsync(request.DeckId, cancellationToken);
            if (deck == null)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, $"Deck med id {request.DeckId} kunde inte hittas"));
            }

            // STEG 3: Hitta och ta bort flashcard
            var flashCard = deck.FlashCards.FirstOrDefault(fc => fc.Id == request.FlashCardId);
            if (flashCard == null)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.FlashCardError.NotFound, 
                        $"Flashcard med id {request.FlashCardId} kunde inte hittas i decket"));
            }

            // STEG 4: Ta bort från decket
            deck.RemoveCard(request.FlashCardId);

            // STEG 5: Spara ändringar
            await _deckRepository.UpdateAsync(deck, cancellationToken);

            return OperationResult.Success();
        }
    }
}

