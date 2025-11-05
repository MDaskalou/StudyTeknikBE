using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.IRepository; 
using Domain.Common;
using FluentValidation;
using MediatR;
using System.Linq;
using System; // <-- Lägg till 'using System' för Exception

namespace Application.FlashCards.Commands.UpdateFlashcardCommand
{
    public class UpdateFlashcardCommandHandler 
        : IRequestHandler<UpdateFlashcardCommand, OperationResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdateFlashcardCommand> _validator;
        private readonly IDeckRepository _deckRepository;

        public UpdateFlashcardCommandHandler(
            ICurrentUserService currentUserService,
            IValidator<UpdateFlashcardCommand> validator, 
            IDeckRepository deckRepository)
        {
            _currentUserService = currentUserService;
            _validator = validator;
            _deckRepository = deckRepository;
        }

        public async Task<OperationResult> Handle(UpdateFlashcardCommand request, CancellationToken ct)
        {
            // 1. Validera (samma som UpdateDeck)
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));                
                return OperationResult.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));                  
            }
            
            // 2. Hämta UserId (samma som UpdateDeck)
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return OperationResult.Failure(Error.Forbidden(ErrorCodes.General.Forbidden, "Användaren är inte auktoriserad."));
            }
            
            // 3. Hämta Deck (samma som UpdateDeck)
            var deck = await _deckRepository.GetByIdTrackedAsync(request.DeckId, ct);
            
            // 4. Validera Owner (samma som UpdateDeck)
            if(deck == null || deck.UserId != userId)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, "Kortleken hittades inte eller så saknas behörighet."));
            }
            
           
            try
            {
                deck.UpdateFlashCard(
                    request.FlashCardId, 
                    request.FrontText, 
                    request.BackText
                );
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return OperationResult.Failure(
                    Error.Validation(ErrorCodes.General.Validation, ex.Message)
                );
            }
                
            await _deckRepository.UpdateAsync(deck, ct);
            
            return OperationResult.Success();
        }
    }
}