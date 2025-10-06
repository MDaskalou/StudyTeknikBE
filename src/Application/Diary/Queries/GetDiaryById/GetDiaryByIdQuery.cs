using Application.Common.Results;
using Application.Diary.Dtos;
using MediatR;

namespace Application.Diary.Queries.GetDiaryById
{
    public sealed record GetDiaryByIdQuery(Guid Id) : IRequest<OperationResult<GetDiaryByIdDto>>;



}