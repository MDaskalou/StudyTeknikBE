using Application.Common.Results;
using Application.Decks.IRepository;
using Application.FlashCards.Dtos;
using Application.Mapper;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.FlashCards.Queries.GetFlashCardById
{
    public sealed class GetFlashCardByIdQueryHandler 
        : IRequestHandler<GetFlashCardByIdQuery, OperationResult<FlashCardDto>>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly IValidator<GetFlashCardByIdQuery> _validator;

        public GetFlashCardByIdQueryHandler(
            IDeckRepository deckRepository,
            IValidator<GetFlashCardByIdQuery> validator)
        {
            _deckRepository = deckRepository;
            _validator = validator;
        }

        public async Task<OperationResult<FlashCardDto>> Handle(
            GetFlashCardByIdQuery request, CancellationToken cancellationToken)
        {
            // STEG 1: Validera
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<FlashCardDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEG 2: Hämta decket
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, cancellationToken);
            if (deck == null)
            {
                return OperationResult<FlashCardDto>.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, $"Deck med id {request.DeckId} kunde inte hittas"));
            }

            // STEG 3: Hitta flashcard i decket
            var flashCard = deck.FlashCards.FirstOrDefault(fc => fc.Id == request.FlashCardId);
            if (flashCard == null)
            {
                return OperationResult<FlashCardDto>.Failure(
                    Error.NotFound(ErrorCodes.FlashCardError.NotFound, 
                        $"Flashcard med id {request.FlashCardId} kunde inte hittas i decket"));
            }

            // STEG 4: Mappa och returnera
            var dto = flashCard.ToDto();
            return OperationResult<FlashCardDto>.Success(dto);
        }
    }
}

