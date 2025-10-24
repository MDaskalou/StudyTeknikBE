using Application.Decks.Commands.UpdateDecks;
using Application.Decks.IRepository;
using FluentValidation;

namespace Application.Decks.Commands.UpdateDetailsDeck
{
    public sealed class UpdateDetailsDeckCommandValidator : AbstractValidator<UpdateDetailsDeckCommand>
    {
        
        public UpdateDetailsDeckCommandValidator()
        {
            RuleFor(d => d.PatchDoc)
                .NotNull().WithMessage("Patch-dokumentet får inte vara null.")
                .Must(patchDoc => patchDoc.Operations.Any()
                ).WithMessage("Patch-dokumentet måste innehålla minst en operation.")
                .When(c => c.PatchDoc != null);
            
            RuleFor(d=> d.DeckId)
                .NotEmpty().WithMessage("Id får inte vara tomt.");
        }
    }
}