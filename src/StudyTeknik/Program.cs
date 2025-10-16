using Application;
using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Security;
using Application.Student.Queries.GetAllStudents;
using Infrastructure.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using StudyTeknik.Middleware;
using StudyTeknik.Service;
using MediatR;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Linq;
using Application.Student.Repository;
using Infrastructure.Persistence.Repositories;
using System.IdentityModel.Tokens.Jwt;
using Application.Abstractions.IPersistence;

namespace StudyTeknik;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // JWT-konfiguration
        var authority = builder.Configuration["Jwt:Authority"];
        var audience  = builder.Configuration["Jwt:Audience"];

        Console.WriteLine("--- JWT CONFIGURATION ---");
        Console.WriteLine($"Authority: {authority}");
        Console.WriteLine($"Audience:  {audience}");
        Console.WriteLine("-------------------------");

        // Custom HttpClientHandler (SSL i dev)
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    Console.WriteLine($"🔍 SSL Validering för {message.RequestUri} ignoreras i Development-läge.");
                    return true;
                }
                return errors == System.Net.Security.SslPolicyErrors.None;
            }
        };

        // --- MANUELL NYCKELHÄMTNING ---
        ICollection<SecurityKey>? signingKeys = null;
        if (!string.IsNullOrEmpty(authority))
        {
            try
            {
                Console.WriteLine("Development mode: Manuellt hämtar signeringsnycklar...");

                // Skapa HttpClient ovanpå din handler
                var httpClient = new HttpClient(httpClientHandler);

                // HttpDocumentRetriever med HttpClient
                var httpDocumentRetriever = new HttpDocumentRetriever(httpClient)
                {
                    RequireHttps = !builder.Environment.IsDevelopment()
                };

                // Hämta discovery document
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    httpDocumentRetriever
                );

                Console.WriteLine($"Steg 1: Hämtar discovery document från {configurationManager.MetadataAddress}");
                var discoveryDocument = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                Console.WriteLine("✅ Discovery document hämtat.");

                // Hämta nycklar
                Console.WriteLine($"Steg 2: Hämtar JWKS från {discoveryDocument.JwksUri}");
                signingKeys = discoveryDocument.SigningKeys;
                Console.WriteLine($"✅ {signingKeys.Count} signeringsnycklar hämtade från IdP.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Kritiskt fel: Kunde inte hämta signeringsnycklar från IdP. Applikationen kan inte starta säkert.");
                Console.WriteLine($"   Felmeddelande: {ex.Message}");
                Console.ResetColor();
                return;
            }
        }
        // --- SLUT MANUELL NYCKELHÄMTNING ---
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        // AuthN
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience  = audience;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = "roles",
                    NameClaimType = "sub"
                    // IssuerSigningKeys sätts nedan om vi har nycklar
                };

                // Sätt endast om nycklar fanns
                if (signingKeys?.Count > 0)
                {
                    options.TokenValidationParameters.IssuerSigningKeys = signingKeys;
                }

                if (builder.Environment.IsDevelopment())
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Auth failed: {context.Exception.GetType().Name}");
                            Console.WriteLine($"   Message: {context.Exception.Message}");
                            Console.ResetColor();
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ Token validated successfully");
                            var userId = context.Principal?.FindFirst("sub")?.Value;
                            var roles = context.Principal?.FindAll("roles").Select(c => c.Value) ?? Enumerable.Empty<string>();
                            Console.WriteLine($"   User:  {userId}");
                            Console.WriteLine($"   Roles: {string.Join(", ", roles)}");
                            Console.ResetColor();
                            return Task.CompletedTask;
                        }
                    };
                }
            });

        // Services
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        //Repositoy
        builder.Services.AddScoped<IStudentRepository, StudentRepository>();
        builder.Services.AddScoped<IDiaryRepository, DiaryRepository>(); 
        
        //AIService
        builder.Services.AddScoped<IAIService, AIService>();


        // AuthZ
       builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasWriteScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context => 
              {
                  Console.WriteLine("--- Utvärderar 'HasWriteScope' Policy ---");
                  // Leta efter ett claim som heter antingen "scope" eller det långa standardnamnet.
                  var scopeClaim = context.User.Claims.FirstOrDefault(c => c.Type == "scope" || c.Type == "http://schemas.microsoft.com/identity/claims/scope");

                  if (scopeClaim == null)
                  {
                      Console.ForegroundColor = ConsoleColor.Red;
                      Console.WriteLine("--> FEL: 'scope'-claim hittades INTE!");
                      Console.ResetColor();
                      // Skriv ut alla claims vi faktiskt hittade, för att se vad som är fel.
                      Console.WriteLine("    Tillgängliga claims i token:");
                      foreach(var claim in context.User.Claims)
                      {
                          Console.WriteLine($"      - Typ: '{claim.Type}', Värde: '{claim.Value}'");
                      }
                      return false; // Misslyckas authoriseringen
                  }

                  Console.ForegroundColor = ConsoleColor.Green;
                  Console.WriteLine($"--> SUCCÉ: Hittade 'scope'-claim! Typ='{scopeClaim.Type}', Värde='{scopeClaim.Value}'");
                  var scopes = scopeClaim.Value.Split(' ');
                  var hasScope = scopes.Contains("diary:write");
                  Console.WriteLine($"--> Innehåller den 'diary:write'? {hasScope}");
                  Console.ResetColor();

                  return hasScope; // Returnera true om scopet finns, annars false.
              }));

    // Gör samma sak för läs-behörighet
    options.AddPolicy("HasReadScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context => 
                  context.User.HasClaim(c => 
                      (c.Type == "scope" || c.Type == "http://schemas.microsoft.com/identity/claims/scope") && 
                      c.Value.Split(' ').Contains("diary:read")
                  )
              ));
});
        
        // MediatR & FluentValidation
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllStudentsHandler).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(GetAllStudentsHandler).Assembly);

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Skriv: Bearer {token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} }
            });
        });

        var app = builder.Build();

        // Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            using var scope = app.Services.CreateScope();
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            await DatabaseSeeder.SeedAsync(db, logger);
        }

        app.UseHttpsRedirection();
        app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ForbiddenLoggingMiddleware>();
        app.MapControllers();

        app.Run();
    }
}
