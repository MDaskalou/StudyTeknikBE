#nullable enable
using System.Security.Claims;
using Domain.Common; 

namespace StudyTeknik.Middleware
{
    // ANSVAR: Skapa en fejkad ClaimsPrincipal i Development
    public class DevelopmentAuthenticationMiddleware 
    {
        private readonly RequestDelegate _next;
        private static readonly Guid TestStudentId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        private const string TestStudentName = "Sam Student";
        private const string TestStudentEmail = "student@demo.local";
        private const string TestStudentExternalId = "student";

        public DevelopmentAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skapa bara om ingen redan är inloggad
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, TestStudentId.ToString()), 
            
                    // Detta används av UserProvisioningMiddleware
                    new Claim("sub", TestStudentExternalId), 
            
                    new Claim(ClaimTypes.Name, TestStudentName),
                    new Claim(ClaimTypes.Email, TestStudentEmail),
                    new Claim(ClaimTypes.Role, Role.Student.ToString())
                };
                var identity = new ClaimsIdentity(claims, "FakeDevelopmentAuth");
                var principal = new ClaimsPrincipal(identity);
                context.User = principal; // Sätt den fejkade användaren
            }
            await _next(context); // Gå vidare
        }
    }

    // Extension method (är denna korrekt definierad?)
    public static class DevelopmentAuthenticationExtensions 
    {
        public static IApplicationBuilder UseDevelopmentAuthentication(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DevelopmentAuthenticationMiddleware>();
        }
    }
}