using FluentValidation;

namespace Application.AI.Commands.Rewrite
{
    public class RewriteTextCommandValidator :AbstractValidator<RewriteTextCommand>
    {
        public RewriteTextCommandValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Text får inte vara tom")
                .MaximumLength(5000).WithMessage("Texten är för lång (max 5000 tecken).");
            
        }
    }
}