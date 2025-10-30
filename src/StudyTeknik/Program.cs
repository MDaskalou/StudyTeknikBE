#nullable enable
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
using Application.Decks.IRepository;

namespace StudyTeknik;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // --- NY KOD (Villkorlig autentisering) ---
        
            // JWT-konfiguration
            var authority = builder.Configuration["Jwt:Authority"];
            var audience  = builder.Configuration["Jwt:Audience"];

            Console.WriteLine("--- JWT CONFIGURATION ---");
            Console.WriteLine($"Authority: {authority}");
            Console.WriteLine($"Audience:  {audience}");
            Console.WriteLine("-------------------------");

            // Custom HttpClientHandler (SSL i dev)
            var httpClientHandler = new HttpClientHandler(); // Enkel för produktion

            // --- MANUELL NYCKELHÄMTNING ---
            ICollection<SecurityKey>? signingKeys = null;
            if (!string.IsNullOrEmpty(authority))
            {
                try
                {
                    // (Din manuella nyckelhämtning är här, inga ändringar behövs inuti)
                    var httpClient = new HttpClient(httpClientHandler);
                    var httpDocumentRetriever = new HttpDocumentRetriever(httpClient) { RequireHttps = true };
                    var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"{authority}/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever(),
                        httpDocumentRetriever
                    );
                    var discoveryDocument = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                    signingKeys = discoveryDocument.SigningKeys;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Kritiskt fel: Kunde inte hämta signeringsnycklar från IdP.");
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
                    options.RequireHttpsMetadata = true;

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
                    };

                    if (signingKeys?.Count > 0)
                    {
                        options.TokenValidationParameters.IssuerSigningKeys = signingKeys;
                    }
                });
        
        
        // Services (inga ändringar här)
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        //Repositoy (inga ändringar här)
        builder.Services.AddScoped<IStudentRepository, StudentRepository>();
        builder.Services.AddScoped<IDiaryRepository, DiaryRepository>(); 
        builder.Services.AddScoped<IDeckRepository, DeckRepository>();
        
        //AIService (inga ändringar här)
        builder.Services.AddScoped<IAIService, AIService>();

        // AuthZ (inga ändringar här)
        builder.Services.AddAuthorization(options =>
        {
            // ... (dina 'HasWriteScope' och 'HasReadScope' policies är här, inga ändringar) ...
        });
        
        // MediatR & FluentValidation (inga ändringar här)
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllStudentsHandler).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(GetAllStudentsHandler).Assembly);

        // Swagger (inga ändringar här)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Skriv: Bearer {token}"
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
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            await DatabaseSeeder.SeedAsync(db, logger);
        }

        app.UseHttpsRedirection();
        app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
       // if (app.Environment.IsDevelopment())
       // {
       //     app.UseDevelopmentAuthentication();
            
        //}
        //else
        //{
            //app.UseAuthentication(); 
       // }
       app.UseAuthentication(); 
       app.UseMiddleware<UserProvisioningMiddleware>();
        app.UseAuthorization(); 
        // --- SLUT PÅ NY KOD ---
        
        app.UseMiddleware<ForbiddenLoggingMiddleware>();
        app.MapControllers();

        app.Run();
    }
}