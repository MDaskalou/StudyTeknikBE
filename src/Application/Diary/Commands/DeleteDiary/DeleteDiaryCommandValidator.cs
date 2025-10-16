using FluentValidation;

namespace Application.Diary.Commands.DeleteDiary
{
    public class DeleteDiaryCommandValidator : AbstractValidator<DeleteDiaryCommand>
    {
        public DeleteDiaryCommandValidator()
        {
            RuleFor(d => d.UserId).NotEmpty();  
        }
    }
}