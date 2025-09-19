using System.Security.Claims;
using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Http;

namespace StudyTeknik.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        private ClaimsPrincipal User => _http.HttpContext?.User ?? new ClaimsPrincipal();

        public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

        public Guid? UserId
        {
            get
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;
                return Guid.TryParse(id, out var g) ? g : null;
            }
        }

        public string? RoleName =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;

        public bool IsInRole(string roleName) =>
            User.IsInRole(roleName) ||
            string.Equals(RoleName, roleName, StringComparison.OrdinalIgnoreCase);
    }
}