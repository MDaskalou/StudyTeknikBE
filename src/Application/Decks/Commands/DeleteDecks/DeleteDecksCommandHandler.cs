using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.IRepository;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Decks.Commands.DeleteDecks
{
    public class DeleteDecksCommandHandler: IRequestHandler<DeleteDecksCommand, OperationResult>
    {
        IValidator<DeleteDecksCommand> _validator;
        IDeckRepository _deckRepository;
        ICurrentUserService _currentUserService;
        
        public DeleteDecksCommandHandler(
            IValidator<DeleteDecksCommand> validator,
            IDeckRepository deckRepository,
            ICurrentUserService currentUserService)
        {
            _validator = validator;
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult> Handle(DeleteDecksCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)); 
                return OperationResult.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
                
            }
            
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.General.Forbidden, "Användaren är inte inloggad." ));
            }
            
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, ct);
            if (deck == null)
            {
                return OperationResult.Failure(Error.NotFound(
                    ErrorCodes.General.NotFound,"Decken hittades inte."));
            }
            
            await _deckRepository.DeleteAsync(request.DeckId, ct);
            return OperationResult.Success();
            
            
        }
    }
}