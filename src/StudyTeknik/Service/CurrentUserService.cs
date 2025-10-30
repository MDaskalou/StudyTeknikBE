using Application.Abstractions;
using System;
using System.Security.Claims;
using Application.Abstractions.IPersistence.Repositories;
using Microsoft.AspNetCore.Http;

namespace StudyTeknik.Service
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        
        // Vårt INTERNA Guid-ID. Startar som null.
        public Guid? UserId { get; private set; }

        // Metoden som UserProvisioningMiddleware anropar
        public void SetUserId(Guid id)
        {
            if (UserId is null)
            {
                UserId = id;
            }
        }
        
        // --- 2. Implementation av "Läsare" ---

        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        private ClaimsPrincipal User => _http.HttpContext?.User ?? new ClaimsPrincipal();

        // Läser det EXTERNA ID:t från "sub"-claimet
        public string? ExternalId => User.FindFirst("sub")?.Value;

        // Läser Roll-namnet
        public string? RoleName =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
    }
}