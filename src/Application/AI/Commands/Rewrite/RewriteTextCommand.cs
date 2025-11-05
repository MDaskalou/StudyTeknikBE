using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;

namespace Application.AI.Commands.Rewrite
{
    public sealed record RewriteTextCommand(string Text): IRequest<OperationResult<RewriteResponseDto>>;
}