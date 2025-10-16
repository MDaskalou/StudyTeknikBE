using Application.Common.Results;
using Application.Diary.Dtos;
using MediatR;

namespace Application.Diary.Commands.CreateDiary
{
    public sealed record CreateDiaryCommand(
        string UserId,
        string? Name,
        string? Email,
        DateOnly EntryDate,
        string Text) : 
        IRequest<OperationResult<CreateDiaryEntryDto>>;
    
        
    
}