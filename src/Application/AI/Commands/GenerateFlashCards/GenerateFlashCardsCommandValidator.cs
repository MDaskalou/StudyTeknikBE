using FluentValidation;

namespace Application.AI.Commands.GenerateFlashCards
{
    public class GenerateFlashCardsCommandValidator : AbstractValidator<GenerateFlashCardsCommand>
    {
        public GenerateFlashCardsCommandValidator()
        {
            RuleFor(x => x.PdfContent)
                .NotEmpty().WithMessage("PDF-innehål kan inte vara tom")
                .MaximumLength(50000).WithMessage("PDF-innehål är för långt (max 50 000 tecken)");

            RuleFor(x => x.DeckId)
                .NotEqual(Guid.Empty).WithMessage("DeckId kan inte vara tom");
        }
    }
}

