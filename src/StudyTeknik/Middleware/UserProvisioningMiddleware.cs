#nullable enable
using System.Security.Claims;
using Application.Student.Repository;
using Domain.Entities;
using Domain.Common;

namespace StudyTeknik.Middleware
{
    // ANSVAR: Se till att en UserEntity finns i databasen
    public class UserProvisioningMiddleware 
    {
        private readonly RequestDelegate _next;

        public UserProvisioningMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStudentRepository studentRepository)
        {
            // Kör bara om användaren ÄR autentiserad (antingen fejkad eller riktig)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var externalId = context.User.FindFirstValue("sub"); // Hämta externa IDt

                if (!string.IsNullOrEmpty(externalId))
                {
                    // Kolla om användaren finns
                    var userEntity = await studentRepository.GetByExternalIdAsync(externalId, context.RequestAborted);

                    // Skapa bara om den INTE finns
                    if (userEntity == null)
                    {
                        // --- KONTROLLERA OCH KOMPLETTERA DETTA BLOCK ---
                        var newUser = new UserEntity
                        {
                            Id = Guid.NewGuid(), // Sätt ett internt Guid
                            ExternalSubject = externalId, // Från context.User.FindFirstValue("sub")

                            // Hämta från claims, med fallback-värden om claim saknas
                            FirstName = context.User.FindFirstValue(ClaimTypes.GivenName) ?? "Okänt", // Eller "Ny"
                            LastName = context.User.FindFirstValue(ClaimTypes.Surname) ?? "Användare", // Eller "Namn"
                            Email = context.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty, // Viktigt med ?? string.Empty för att undvika NULL
        
                            Role = Role.Student, // Sätt standardroll
                            CreatedAtUtc = DateTime.UtcNow,
                            UpdatedAtUtc = DateTime.UtcNow,
                            ExternalProvider = "Logto", // Eller hämta från 'iss'-claim om det finns

                            // Säkerställ att fälten från UserEntity som INTE kommer från claims
                            // också får vettiga default-värden (om de inte tillåter null i databasen)
                            SecurityNumber = string.Empty, // Om den inte får vara null
                            ConsentGiven = false, // Default-värde
                            // ConsentGivenAtUtc lämnas null (eftersom den är nullable DateTime?)
                            // ConsentSetBy lämnas null (eftersom den är nullable string?)
                        };
                        // --- SLUT PÅ KONTROLL ---

                        // Anropa metoden som sparar direkt
                        await studentRepository.AddEntityAsync(newUser, context.RequestAborted); 
                    }
                }
            }
            await _next(context); // Gå vidare
        }
    }
}