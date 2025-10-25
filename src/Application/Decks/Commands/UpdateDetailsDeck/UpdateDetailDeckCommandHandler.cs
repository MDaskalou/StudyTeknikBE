using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository;
using Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.Decks.Commands.UpdateDetailsDeck
{
    public sealed class UpdateDetailDeckCommandHandler : IRequestHandler<UpdateDetailsDeckCommand, OperationResult>
    {
        private readonly IValidator<UpdateDetailsDeckCommand> _validator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDeckRepository _deckRepository;
        
        public UpdateDetailDeckCommandHandler(
            ICurrentUserService currentUserService,
            IValidator<UpdateDetailsDeckCommand> validator,
            IDeckRepository deckRepository)
        {
            _currentUserService = currentUserService;
            _validator = validator;
            _deckRepository = deckRepository;
        }

        public async Task<OperationResult> Handle(UpdateDetailsDeckCommand request, CancellationToken ct)
        {
            var commandValidationResult = await _validator.ValidateAsync(request, ct);
            if (!commandValidationResult.IsValid)
            {
                var errorMessages = string.Join(", ", commandValidationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return OperationResult.Failure(Error.Forbidden(ErrorCodes.General.Forbidden, "Användaren är inte inloggad."));
            }

            var deck = await _deckRepository.GetByIdTrackedAsync(request.DeckId, ct);

            if (deck == null || deck.UserId != userId.Value)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.DeckError.NotFound, "Kortleken hittades inte eller så saknas behörighet."));
            }


            var deckPatchDto = new UpdateDetailsDeckDto
            {
                Title = deck.Title,
                CourseName = deck.CourseName,
                SubjectName = deck.SubjectName
            };
            
            var applyErrors = new List<JsonPatchError>();

            request.PatchDoc.ApplyTo(deckPatchDto, error => applyErrors.Add(error));

            if (applyErrors.Any())
            {
                var errorMessages = string.Join(", ", applyErrors.Select(e => $"{e.Operation.path}: {e.ErrorMessage}"));
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, $"Fel vid applicering av patch: {errorMessages}"));
            }
            

            if (deckPatchDto.Title != deck.Title)
            {
                deck.SetTitle(deckPatchDto.Title);
            }
            if (deckPatchDto.CourseName != deck.CourseName || deckPatchDto.SubjectName != deck.SubjectName)
            {
                deck.SetCategories(deckPatchDto.CourseName, deckPatchDto.SubjectName);
            }

            await _deckRepository.UpdateAsync(deck, ct); 

            return OperationResult.Success();
            
        }
        
    }   
}