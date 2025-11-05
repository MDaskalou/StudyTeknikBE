using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.IRepository;
using Application.FlashCards.Dtos;
using Application.Mapper;
using Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.FlashCards.Commands
{
    public class AddFlashcardToDeckCommandHandler : IRequestHandler<AddFlashcardToDeckCommand, OperationResult<FlashCardDto>>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<AddFlashcardToDeckCommand> _validator;
        
        public AddFlashcardToDeckCommandHandler(IDeckRepository deckRepository, ICurrentUserService currentUserService,
            IValidator<AddFlashcardToDeckCommand> validator)
        {
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
            _validator = validator;
        }

        public async Task<OperationResult<FlashCardDto>> Handle(AddFlashcardToDeckCommand request,
            CancellationToken cancellationToken)
        {
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, cancellationToken);
            if (deck == null)
            {
                return OperationResult<FlashCardDto>.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, $"Deck med id {request.DeckId} kunde inte hittas"));
            }
            
            var userId = _currentUserService.UserId;

            if (userId is null || deck.UserId != userId)
            {
                return OperationResult<FlashCardDto>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Användare är inte auktoriserad att ändra detta deck"));
            }
            
            var newFlashCard= deck.AddFlashCard(request.FrontText, request.BackText);
            
            await _deckRepository.UpdateAsync(deck, cancellationToken);
            
            var dto = newFlashCard.ToDto();
            
            return OperationResult<FlashCardDto>.Success(dto);
        }
    }
}