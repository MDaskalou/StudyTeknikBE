using FluentValidation;

namespace Application.Diary.Queries.GetDiaryById
{
    public sealed class GetDiaryByIdQueryValidator : AbstractValidator<GetDiaryByIdQuery>
    {
        public GetDiaryByIdQueryValidator()
        {
            RuleFor(d => d.Id).NotEmpty().WithMessage("Id för dagboken får inte vara tomt.");
        }
    }
        
    
}