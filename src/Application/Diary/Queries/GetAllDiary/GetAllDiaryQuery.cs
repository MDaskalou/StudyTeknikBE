using Application.Common.Results;
using Application.Diary.Dtos;
using MediatR;

namespace Application.Diary.Queries.GetAllDiary
{
    public sealed record GetAllDiaryQuery : IRequest<OperationResult<IReadOnlyList<GetAllDiaryDto>>>;
    
        
    
}