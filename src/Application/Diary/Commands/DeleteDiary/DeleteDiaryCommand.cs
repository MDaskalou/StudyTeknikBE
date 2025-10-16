using Application.Common.Results;
using MediatR;

namespace Application.Diary.Commands.DeleteDiary
{
    public sealed record DeleteDiaryCommand(Guid DiaryEntityId, string UserId) : IRequest<OperationResult>;

}