using Application;
using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Security;
using Application.Student.Queries.GetAllStudents;
using Infrastructure.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StudyTeknik.Middleware;
using StudyTeknik.Service;
using MediatR;
using FluentValidation;

namespace StudyTeknik;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers()
            .AddNewtonsoftJson();

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure(builder.Configuration);

        // HttpClient för metadata-hämtning
        builder.Services.AddHttpClient();

        // HttpClientHandler som accepterar SSL-certifikat i development
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    return true; // Acceptera alla certifikat i development
                }
                return errors == System.Net.Security.SslPolicyErrors.None;
            }
        };

        // Hämta JWT-konfiguration
        var authority = builder.Configuration["Jwt:Authority"];
        var audience = builder.Configuration["Jwt:Audience"];

        Console.WriteLine("--- JWT CONFIGURATION ---");
        Console.WriteLine($"Authority: {authority}");
        Console.WriteLine($"Audience: {audience}");
        Console.WriteLine($"Metadata: {authority}/.well-known/openid-configuration");
        Console.WriteLine("-------------------------");

        // Konfigurera JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
                options.BackchannelHttpHandler = httpClientHandler;
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = true,
                    ValidAudience = audience, 
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = "roles",
                    NameClaimType = "sub", 
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                // Development logging
                if (builder.Environment.IsDevelopment())
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"❌ Auth failed: {context.Exception.GetType().Name}");
                            Console.WriteLine($"   Message: {context.Exception.Message}");
                            if (context.Exception.InnerException != null)
                            {
                                Console.WriteLine($"   Inner: {context.Exception.InnerException.Message}");
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("✅ Token validated successfully");
                            var userId = context.Principal?.FindFirst("sub")?.Value;
                            var roles = context.Principal?.FindAll("roles").Select(c => c.Value);
                            Console.WriteLine($"   User: {userId}");
                            Console.WriteLine($"   Roles: {string.Join(", ", roles ?? Array.Empty<string>())}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"⚠️ Auth challenge: {context.Error}");
                            Console.WriteLine($"   Description: {context.ErrorDescription}");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            Console.WriteLine("📨 Token received from request");
                            return Task.CompletedTask;
                        }
                    };
                }
            });

        // CurrentUserService
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Authorization policies
        builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy("Users.Manage", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    ctx.User.IsInRole("Admin")
                    || ctx.User.HasClaim(c =>
                        c.Type == "scope" &&
                        c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Contains("users:manage"))
                );
            });

            opts.AddPolicy("Users.Read", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    ctx.User.IsInRole("Admin") ||
                    ctx.User.IsInRole("Teacher") ||
                    ctx.User.IsInRole("Mentor") ||
                    ctx.User.IsInRole("Student")
                    || ctx.User.HasClaim(c =>
                        c.Type == "scope" &&
                        c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Contains("users:read"))
                );
            });

            opts.AddPolicy("Classes.Read", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == "scope" &&
                                           c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Contains("classes:read")));
            });

            opts.AddPolicy("Classes.Manage", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == "scope" &&
                                           c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Contains("classes:manage")));
            });

            opts.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
            opts.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
            opts.AddPolicy("MentorOnly", policy => policy.RequireRole("Mentor"));
            opts.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        // MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllStudentsHandler).Assembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStudentHandler).Assembly));

        // FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(CreateStudentHandler).Assembly);

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            if (builder.Environment.IsDevelopment())
            {
                c.AddSecurityDefinition("X-UserId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "X-UserId",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Description = "Dev auth: seed GUID för användare"
                });

                c.AddSecurityDefinition("X-Role", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "X-Role",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Description = "Dev auth: Student | Teacher | Mentor | Admin"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "X-UserId"
                            }
                        },
                        new string[] { }
                    },
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "X-Role"
                            }
                        },
                        new string[] { }
                    }
                });
            }
            else
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Skriv: Bearer {token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            }
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            // Seed testdata
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            await DatabaseSeeder.SeedAsync(db, logger);

            // Testa metadata endpoint
            try
            {
                using var testClient = new HttpClient(httpClientHandler);
                var metadataUrl = $"{authority}/.well-known/openid-configuration";
                Console.WriteLine($"\n🔍 Testing metadata endpoint: {metadataUrl}");

                var response = await testClient.GetAsync(metadataUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Metadata endpoint is reachable");
                    Console.WriteLine($"   Response length: {content.Length} characters\n");
                }
                else
                {
                    Console.WriteLine($"❌ Metadata endpoint failed: {response.StatusCode}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Cannot reach metadata endpoint: {ex.Message}\n");
            }
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ForbiddenLoggingMiddleware>();
        app.MapControllers();

        app.Run();
    }
}