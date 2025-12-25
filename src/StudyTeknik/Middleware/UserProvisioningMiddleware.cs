﻿#nullable enable
using System.Security.Claims;
using Application.Abstractions.IPersistence.Repositories;
using Application.Student.Repository;
using Domain.Entities;
using Domain.Common;
using StudyTeknik.Extensions;

namespace StudyTeknik.Middleware
{
    /// <summary>
    /// Middleware som säkerställer att en UserEntity finns i databasen för varje autentiserad request.
    /// Lägger också till Internal GUID som claim så controllern kan enkelt hämta det.
    /// </summary>
    public class UserProvisioningMiddleware 
    {
        private readonly RequestDelegate _next;
        private const string InternalUserIdClaimType = "InternalUserId";

        public UserProvisioningMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // STEG 1: Hämta externt ID från claims
                var externalId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                UserEntity? userEntity = null;

                if (!string.IsNullOrEmpty(externalId))
                {
                    // STEG 2: Hämta eller skapa användaren
                    userEntity = await studentRepository.GetByExternalIdAsync(externalId, context.RequestAborted);

                    if (userEntity == null)
                    {
                        // Konvertera externt ID till deterministisk GUID
                        var userId = context.User.GetUserIdAsGuid() ?? Guid.NewGuid();
                        
                        var newUser = new UserEntity
                        {
                            Id = userId,
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

                    // STEG 3: 🔐 KRITISKT - Lägg till Internal GUID som claim
                    // Detta säkerställer att GetUserIdAsGuid() kan hitta det snabbt senare
                    if (userEntity != null)
                    {
                        var claimsIdentity = context.User.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            // Lägg till Internal GUID som claim
                            claimsIdentity.AddClaim(new Claim(InternalUserIdClaimType, userEntity.Id.ToString()));
                        }

                        // STEG 4: Sätt UserId på CurrentUserService också
                        currentUserService.SetUserId(userEntity.Id);
                    }
                }
            }
            
            await _next(context); 
        }
    }
}