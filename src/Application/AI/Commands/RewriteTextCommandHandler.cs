#nullable enable
using Application.Common.Results;
using Domain.Common; 
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.IPersistence;
using Application.AI.Dtos;

namespace Application.AI.Commands.RewriteText
{
    public sealed class RewriteTextCommandHandler
        : IRequestHandler<RewriteTextCommand, OperationResult<RewriteResponseDto>>
    {
        private readonly IAIService _aiService;
        private readonly IValidator<RewriteTextCommand> _validator;

        public RewriteTextCommandHandler(
            IAIService aiService,
            IValidator<RewriteTextCommand> validator)
        {
            _aiService = aiService;
            _validator = validator;
        }

        public async Task<OperationResult<RewriteResponseDto>> Handle(
            RewriteTextCommand request, CancellationToken cancellationToken)
        {
            // STEG 1: Validera
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<RewriteResponseDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEG 2: Anropa AI-tjänsten
            // Logiken från controllern flyttas hit
            var rewrittenText = await _aiService.RewriteDiaryEntryAsync(request.Text, cancellationToken);

            // STEG 3: Hantera fel från AI-tjänsten
            if (rewrittenText.StartsWith("Kunde inte generera text"))
            {
                // Returnera ett InternalServiceError
                return OperationResult<RewriteResponseDto>.Failure(
                    Error.InternalServiceError(ErrorCodes.General.InternalServiceError, rewrittenText));
            }

            // STEG 4: Mappa till DTO och returnera Success
            var responseDto = new RewriteResponseDto(rewrittenText);
            return OperationResult<RewriteResponseDto>.Success(responseDto);
            
        }
    }
}