#nullable enable
using System.Security.Claims;
using Application.Abstractions.IPersistence.Repositories;
using Application.Student.Repository;
using Domain.Entities;
using Domain.Common;

namespace StudyTeknik.Middleware
{
    // ANSVAR: Se till att en UserEntity finns i databasen
    public class UserProvisioningMiddleware 
    {
        private readonly RequestDelegate _next;
        private const string InternalUserIdKey = "InternalUserId";

        public UserProvisioningMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Injicera ICurrentUserService i metoden:
        public async Task InvokeAsync(HttpContext context, IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // STEG 1: Läs EXTERNT ID från vår service (istället för HttpContext)
                var externalId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                UserEntity? userEntity = null;

                if (!string.IsNullOrEmpty(externalId))
                {
                    // STEG 2: Hämta/skapa användaren (din kod är bra här)
                    userEntity = await studentRepository.GetByExternalIdAsync(externalId, context.RequestAborted);

                    if (userEntity == null)
                    {
                        // ... (all din logik för att skapa 'newUser' är bra) ...
                        var newUser = new UserEntity
                        {
                            Id = Guid.NewGuid(),
                            ExternalSubject = externalId,
                            FirstName = context.User.FindFirst(ClaimTypes.GivenName)?.Value ?? "Okänt",
                            LastName = context.User.FindFirst(ClaimTypes.Surname)?.Value ?? "Användare",
                            Email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                            Role = Role.Student,
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow,
                            ExternalProvider = "Logto", 
                            SecurityNumber = string.Empty,
                            ConsentGiven = false,
                        };
                
                        await studentRepository.AddEntityAsync(newUser, context.RequestAborted);
                        userEntity = newUser;
                    }

                    // STEG 3: SÄTT det INTERNA ID:t på vår service
                    if (userEntity != null) 
                    {
                        currentUserService.SetUserId(userEntity.Id);
                    }
                }
            }
            await _next(context); 
        }
    }
}