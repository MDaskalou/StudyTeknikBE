using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository;
using Application.Decks.Queries.GetDeckById;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Decks.Commands.UpdateDecks
{
    public class UpdateDeckCommandHandler : IRequestHandler<UpdateDeckCommand, OperationResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<UpdateDeckCommand> _validator;
        private readonly IDeckRepository _deckRepository;
        
        public UpdateDeckCommandHandler(
            ICurrentUserService currentUserService,
            IValidator<UpdateDeckCommand> validator, IDeckRepository deckRepository)
        {
            _currentUserService = currentUserService;
            _validator = validator;
            _deckRepository = deckRepository;
        }

        public async Task<OperationResult> Handle(UpdateDeckCommand request, CancellationToken ct)
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
                return OperationResult<UpdateDeckDto>.Failure(Error.Forbidden(ErrorCodes.General.Forbidden, "Användaren är inte auktoriserad."));
            }
            
            var deck = await _deckRepository.GetByIdTrackedAsync(request.DeckId, ct);
            
            if(deck == null || deck.UserId != userId)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, "Kortleken hittades inte eller så saknas behörighet."));
            }
            
            deck.SetTitle(request.Title);
            deck.SetCategories(request.CourseName, request.SubjectName);
                
            await _deckRepository.UpdateAsync(deck, ct);
            
            return OperationResult.Success();
             
            
            
        }
    }
}