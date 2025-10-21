using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Domain.Models.Users;
using Domain.Common;
using Application.Student.Repository; // VIKTIGT: Importera repository-interfacet

namespace Infrastructure.Middleware
{
    public class UserProvisioningMiddleware
    {
        private readonly RequestDelegate _next;

        public UserProvisioningMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // KORREKT: Vi injicerar kontraktet (IStudentRepository), inte implementationen (DbContext).
        public async Task InvokeAsync(HttpContext context, IStudentRepository studentRepository)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var externalSubjectId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(externalSubjectId))
                {
                    // Steg 1: Fråga repositoryt om användaren finns.
                    // Middlewaren vet inte HUR detta görs, den bara frågar.
                    var existingUser = await studentRepository.GetDomainUserByExternalIdAsync(externalSubjectId, CancellationToken.None);

                    // Steg 2: Om användaren INTE finns, skapa den.
                    if (existingUser is null)
                    {
                        var email = context.User.FindFirstValue(ClaimTypes.Email) ?? "saknas@epost.se";
                        var firstName = context.User.FindFirstValue(ClaimTypes.GivenName) ?? "Okänt";
                        var lastName = context.User.FindFirstValue(ClaimTypes.Surname) ?? "Namn";
                        var securityNumber = context.User.FindFirstValue("ssn") ?? "000000-0000";

                        // Skapa det rika domänobjektet. Detta är middlewarens ansvar.
                        var newUserDomainObject = new User(
                            firstName,
                            lastName,
                            securityNumber,
                            email,
                            Role.Student,
                            "logto",
                            externalSubjectId
                        );
                        
                        // Steg 3: Be repositoryt att spara den nya användaren.
                        // Middlewaren vet inte HUR den sparas (mappning, etc.), den bara ger ordern.
                        await studentRepository.AddAsync(newUserDomainObject, CancellationToken.None);
                    }
                }
            }

            await _next(context);
        }
    }
}