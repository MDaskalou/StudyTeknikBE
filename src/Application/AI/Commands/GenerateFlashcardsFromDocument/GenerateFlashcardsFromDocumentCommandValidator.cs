using FluentValidation;

namespace Application.AI.Commands.GenerateFlashcardsFromDocument
{
    public sealed class GenerateFlashcardsFromDocumentCommandValidator 
        : AbstractValidator<GenerateFlashcardsFromDocumentCommand>
    {
        public GenerateFlashcardsFromDocumentCommandValidator()
        {
            RuleFor(x => x.DeckId)
                .NotEmpty().WithMessage("DeckId får inte vara tomt.");
            
            RuleFor(x => x.FileStream)
                .NotNull().WithMessage("Filströmmen får inte vara null.")
                .Must(stream => stream.Length > 0).WithMessage("Filen verkar vara tom.");
            
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("Filnamnet får inte vara tomt.")
                .Must(name => name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Endast .pdf-filer stöds för närvarande.");
        }
    }
}