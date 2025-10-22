using Application.Common.Results;
using MediatR;

namespace Application.Diary.Commands.UpdateDiary
{
    public sealed record UpdateDiaryCommand(Guid Id, string UserId,string Text) : IRequest<OperationResult>;


}