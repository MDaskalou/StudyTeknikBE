using Application.Abstractions.IPersistence;
using Application.Abstractions.IPersistence.Repositories;
using Application.AI.Dtos;
using Application.Common.Results;
using Application.Decks.IRepository;
using Domain.Common;
using FluentValidation;
using MediatR;
// För IDeckRepository
// För AiGeneratedCardDto
// För IValidator

// För .Select()

namespace Application.AI.Commands.GenerateFlashcardsFromDocument
{
    public class GenerateFlashcardsFromDocumentCommandHandler 
        : IRequestHandler<GenerateFlashcardsFromDocumentCommand, OperationResult<List<AiGeneratedCardDto>>>
    {
        private readonly IAiService _aiService;
        private readonly IDeckRepository _deckRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<GenerateFlashcardsFromDocumentCommand> _validator; // Din validator

        public GenerateFlashcardsFromDocumentCommandHandler(
            IAiService aiService, 
            IDeckRepository deckRepository, 
            ICurrentUserService currentUserService,
            IValidator<GenerateFlashcardsFromDocumentCommand> validator) // Injicera validatorn
        {
            _aiService = aiService;
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
            _validator = validator; // Spara validatorn
        }

        public async Task<OperationResult<List<AiGeneratedCardDto>>> Handle(
            GenerateFlashcardsFromDocumentCommand request, 
            CancellationToken cancellationToken)
        {
            // --- STEG 1: Validera input (samma mönster som dina andra handlers) ---
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<List<AiGeneratedCardDto>>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // --- STEG 2: Validera behörighet ---
            var userId = _currentUserService.UserId;
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, cancellationToken);
            
            if (userId == null || deck == null || deck.UserId != userId)
            {
                return OperationResult<List<AiGeneratedCardDto>>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Behörighet saknas för denna kortlek."));
            }

            try
            {
                // --- STEG 3: Anropa AI:n ---
                var suggestedCards = await _aiService.GenerateFlashcardsFromDocumentAsync(
                    request.FileStream, 
                    request.FileName, 
                    cancellationToken);

                // --- STEG 4: Returnera listan med förslag ---
                return OperationResult<List<AiGeneratedCardDto>>.Success(suggestedCards);
            }
            catch (Exception ex)
            {
                // Fånga alla fel från IAIService (fil-läsning, AI-anrop, JSON-parsning)
                return OperationResult<List<AiGeneratedCardDto>>.Failure(
                    Error.Conflict("Generering", ex.Message));
            }
        }
    }
}