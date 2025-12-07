using System.Security.Claims;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Service
{
    // Denna klass implementerar ALLA delar av interfacet
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        // --- 1. Implementation av "Hållare" (för UserId) ---
        
        // INTERNT ID (startar som null, sätts av middleware)
        public Guid? UserId { get; private set; }

        // Metod som middleware anropar
        public void SetUserId(Guid id)
        {
            if (UserId is null)
            {
                UserId = id;
            }
        }
        
        // --- 2. Implementation av "Läsare" (för Claims) ---

        // Konstruktor som tar emot HttpContext
        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        // Privat hjälp-property
        private ClaimsPrincipal User => _http.HttpContext?.User ?? new ClaimsPrincipal();

        // EXTERNT ID (läses från token)
        public string? ExternalId => User.FindFirst("sub")?.Value;

        // ROLL (läses från token)
        public string? RoleName =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
    }
}