using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.IRepository;
using Application.FlashCards.Dtos;
using Application.Mapper;
using Domain.Common;
using MediatR;

namespace Application.FlashCards.Queries.GetAllFlashCardsForDeckQuery
{
    public class GetAllFlashCardsForDeckQueryHandler 
        : IRequestHandler<GetAllFlashCardsForDeckQuery, OperationResult<List<FlashCardDto>>>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAllFlashCardsForDeckQueryHandler(IDeckRepository deckRepository, 
            ICurrentUserService currentUserService)
        {
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<List<FlashCardDto>>> Handle(
            GetAllFlashCardsForDeckQuery request, 
            CancellationToken cancellationToken)
        {
            // 1. Hämta kortleken (inklusive korten)
            // Din 'GetByIdAsync' hämtar redan kortleken INKLUSIVE flashcards
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, cancellationToken);
            
            if (deck == null)
            {
                return OperationResult<List<FlashCardDto>>.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, 
                        $"Deck med id {request.DeckId} kunde inte hittas"));
            }

            // 2. Kontrollera ägarskap (säkerhet)
            var userId = _currentUserService.UserId;
            if (userId is null || deck.UserId != userId)
            {
                return OperationResult<List<FlashCardDto>>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, 
                        "Användare är inte auktoriserad att se detta deck"));
            }

            // 3. Mappa om ENBART flashcards-listan till DTOs
            var dtos = deck.FlashCards
                .Select(card => card.ToDto())
                .ToList();

            // 4. Returnera framgång
            return OperationResult<List<FlashCardDto>>.Success(dtos);
        }
    }
}