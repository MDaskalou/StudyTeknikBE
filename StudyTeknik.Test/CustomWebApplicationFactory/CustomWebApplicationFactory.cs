using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace StudyTeknik.Test.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // --- 1. DATABAS HANTERING (Befintlig kod) ---
                
                // Hitta konfigurationen för den "riktiga" SQL Server-databasen
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                // Ta bort den riktiga databasen
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Lägg till en InMemory-databas istället
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // --- 2. AUTENTISERING (Ny kod) ---

                // Konfigurera systemet att använda vår "TestAuthHandler" som standard
                // istället för riktiga JWT-tokens.
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });

                // --- 3. SEEDING (Befintlig kod) ---
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }

    // --- HJÄLPKLASS FÖR ATT FEJKA INLOGGNING ---
    // Denna klass skapar en "falsk" inloggad användare för varje anrop
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Här skapar vi användarens identitet
            var claims = new[] 
            { 
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) 
            };
            
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            // Returnera resultatet som "Success" direkt
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}