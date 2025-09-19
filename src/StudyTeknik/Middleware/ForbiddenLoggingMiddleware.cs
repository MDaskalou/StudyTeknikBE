using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;

namespace StudyTeknik.Middleware
{
    public sealed class ForbiddenLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public ForbiddenLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx, IAuditLogger audit, ICurrentUserService current)
        {
            await _next(ctx);
            if (ctx.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                var path = ctx.Request.Path.Value ?? "";
                var payload = $"{{\"path\":\"{path}\",\"role\":\"{current.RoleName}\"}}";
                await audit.LogAsync("RBAC_FORBIDDEN", payload, current.UserId, ctx.TraceIdentifier, ctx.RequestAborted);
            }
        }
    }
}