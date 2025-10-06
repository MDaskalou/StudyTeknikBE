using Application.Common.Results;
using MediatR;

namespace Application.Diary.Commands.DeleteDiary
{
    public sealed record DeleteDiaryCommand(Guid Id) : IRequest<OperationResult>;

}