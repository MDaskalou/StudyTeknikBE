using Application.Common.Results;
using Application.Diary.Dtos;
using MediatR;

namespace Application.Diary.Queries.GetAllDiary
{
    public sealed record GetAllDiaryQuery(string UserId) : IRequest<OperationResult<IReadOnlyList<GetAllDiaryDto>>>;
    
        
    
}