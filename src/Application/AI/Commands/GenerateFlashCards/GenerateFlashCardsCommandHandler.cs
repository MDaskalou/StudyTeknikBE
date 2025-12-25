using Application.Abstractions.IPersistence;
using Application.Abstractions.IPersistence.Repositories;
using Application.AI.Dtos;
using Application.Common.Results;
using Application.Decks.IRepository;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.AI.Commands.GenerateFlashCards
{
    public sealed class GenerateFlashCardsCommandHandler 
        : IRequestHandler<GenerateFlashCardsCommand, OperationResult<GenerateFlashCardsResponseDto>>
    {
        private readonly IAIService _aiService;
        private readonly IValidator<GenerateFlashCardsCommand> _validator;
        private readonly IDeckRepository _deckRepository;
        private readonly ICurrentUserService _currentUserService;

        public GenerateFlashCardsCommandHandler(
            IAIService aiService,
            IValidator<GenerateFlashCardsCommand> validator,
            IDeckRepository deckRepository,
            ICurrentUserService currentUserService)
        {
            _aiService = aiService;
            _validator = validator;
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<GenerateFlashCardsResponseDto>> Handle(
            GenerateFlashCardsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // STEG 1: Validera
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return OperationResult<GenerateFlashCardsResponseDto>.Failure(
                        Error.Validation(ErrorCodes.General.Validation, errorMessages));
                }

                // STEG 2: Kontrollera att deck existerar och tillhör användaren
                var userId = _currentUserService.UserId;
                if (userId == null)
                {
                    return OperationResult<GenerateFlashCardsResponseDto>.Failure(
                        Error.Forbidden(ErrorCodes.General.Forbidden, "Användare är inte inloggad"));
                }

                var deck = await _deckRepository.GetByIdTrackedAsync(request.DeckId, cancellationToken);
                if (deck == null)
                {
                    return OperationResult<GenerateFlashCardsResponseDto>.Failure(
                        Error.NotFound(ErrorCodes.DeckError.NotFound, $"Deck med id {request.DeckId} kunde inte hittas"));
                }

                // Kontrollera att decket tillhör användaren
                if (deck.UserId != userId.Value)
                {
                    return OperationResult<GenerateFlashCardsResponseDto>.Failure(
                        Error.Forbidden(ErrorCodes.DeckError.NotOwnedByUser, "Du äger inte detta deck"));
                }

                // STEG 3: Generera flashcards via AI
                Console.WriteLine($"🤖 Genererar flashcards från {request.PdfContent.Length} tecken...");
                var generatedCards = await _aiService.GenerateFlashCardsAsync(request.PdfContent, cancellationToken);

                if (generatedCards.Count == 0)
                {
                    return OperationResult<GenerateFlashCardsResponseDto>.Failure(
                        Error.InternalServiceError(ErrorCodes.General.InternalServiceError, 
                            "Kunde inte generera flashcards från PDF-innehål"));
                }

                Console.WriteLine($"✅ AI genererade {generatedCards.Count} flashcards");

                // STEG 4: Lägg till korten i decket
                foreach (var card in generatedCards.Cards)
                {
                    Console.WriteLine($"➕ Lägger till: '{card.FrontText}' -> '{card.BackText.Substring(0, Math.Min(30, card.BackText.Length))}...'");
                    deck.AddFlashCard(card.FrontText, card.BackText);
                }

                Console.WriteLine($"💾 Sparar {deck.FlashCards.Count} flashcards till databas...");
                
                // STEG 5: Spara decket
                await _deckRepository.UpdateAsync(deck, cancellationToken);

                Console.WriteLine($"✨ Flashcards sparade framgångsrikt!");

                // STEG 6: Returnera resultatet
                return OperationResult<GenerateFlashCardsResponseDto>.Success(generatedCards);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Handler Error: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }

                throw;
            }
        }
    }
}

