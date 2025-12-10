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
using Domain.Entities;
using Domain.Common;
using Microsoft.AspNetCore.Builder;

namespace StudyTeknik.Test.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<StudyTeknik.Program>
    {
        // Använd samma GUID som din DatabaseSeeder för konsistens
        public static readonly Guid TestUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly string TestUserExternalSubject = "student";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // --- 1. DATABAS HANTERING ---
                
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });

                // --- 2. AUTENTISERING ---

                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });

                // Stäng av HTTPS-redirects i test (kan orsaka problem)
                services.AddHttpsRedirection(options => options.HttpsPort = null);

                // --- 3. SEEDING MED TEST-ANVÄNDARE ---
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    // Skapa en test-student som matchar din UserEntity struktur
                    if (!db.Users.Any(u => u.Id == TestUserId))
                    {
                        var now = DateTime.UtcNow;
                        var testUser = new UserEntity
                        {
                            Id = TestUserId,
                            FirstName = "Test",
                            LastName = "Student",
                            Email = "test.student@demo.local",
                            SecurityNumber = "20000101-1234",
                            Role = Role.Student,
                            ExternalProvider = "test",
                            ExternalSubject = TestUserExternalSubject,
                            ConsentGiven = true, // Aktiverad för test
                            ConsentGivenAtUtc = now,
                            CreatedAtUtc = now,
                            UpdatedAtUtc = now
                        };
                        
                        db.Users.Add(testUser);
                        db.SaveChanges();
                    }
                }
            });
        }
    }

    // --- HJÄLPKLASS FÖR ATT FEJKA INLOGGNING ---
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // KRITISKT: ClaimTypes.NameIdentifier måste matcha ExternalSubject i databasen!
            // Din middleware letar med ClaimTypes.NameIdentifier
            var claims = new[] 
            { 
                new Claim(ClaimTypes.NameIdentifier, CustomWebApplicationFactory.TestUserExternalSubject), // "student"
                new Claim("sub", CustomWebApplicationFactory.TestUserExternalSubject), // "student"
                new Claim(ClaimTypes.Name, "Test Student"),
                new Claim(ClaimTypes.Email, "test.student@demo.local"),
                new Claim(ClaimTypes.Role, "Student"),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "Student")
            };
            
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}