#nullable enable
using System.Security.Claims;
using Domain.Common; // För din Role-enum

namespace StudyTeknik.Middleware
{
    /// <summary>
    /// Middleware som KUNSTIGT sätter en autentiserad användare (ClaimsPrincipal)
    /// på HttpContext när appen körs i Development-läge.
    /// Detta kringgår behovet av riktiga tokens/cookies vid lokal testning (t.ex. Swagger/Postman).
    /// </summary>
    public class DevelopmentAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        // --- DEFINIERA DIN TESTANVÄNDARE HÄR ---
        // Välj ett Guid som finns i din utvecklingsdatabas.
        private static readonly Guid TestStudentId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        private const string TestStudentName = "Sam Student";
        private const string TestStudentEmail = "student@demo.local";
        // Detta är IDt från din IdP (Logto), viktigt för din UserProvisioningMiddleware
        private const string TestStudentExternalId = "dev|fake-student-id";

        public DevelopmentAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                var claims = new List<Claim>
                {
                    // Detta är det interna Guid-IDt. Din ICurrentUserService kan använda detta.
                    new Claim(ClaimTypes.NameIdentifier, TestStudentId.ToString()),
                    
                    // Detta är det externa "sub"-claimet. Din UserProvisioningMiddleware använder detta.
                    new Claim("sub", TestStudentExternalId), 
                    
                    new Claim(ClaimTypes.Name, TestStudentName),
                    new Claim(ClaimTypes.Email, TestStudentEmail),
                    new Claim(ClaimTypes.Role, Role.Student.ToString())
                };

                var identity = new ClaimsIdentity(claims, "FakeDevelopmentAuth");
                var principal = new ClaimsPrincipal(identity);
                context.User = principal;
            }

            await _next(context);
        }
    }

    // Extension method för enkel registrering i Program.cs
    public static class DevelopmentAuthenticationExtensions
    {
        public static IApplicationBuilder UseDevelopmentAuthentication(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DevelopmentAuthenticationMiddleware>();
        }
    }
}