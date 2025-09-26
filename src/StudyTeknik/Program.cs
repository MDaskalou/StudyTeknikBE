using Application;
using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Application.Security;
using Application.Student.Queries.GetAllStudents;
using Domain.Models.Users;
using Infrastructure.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication;
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
        
        

        // Add services to the container.
        builder.Services.AddControllers()
            .AddNewtonsoftJson();
        
        
        builder.Services.AddApplicationServices();

        builder.Services.AddInfrastructure(builder.Configuration);
        
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = StudyTeknik.Auth.DevAuthHandler.Scheme;
                    options.DefaultChallengeScheme    = StudyTeknik.Auth.DevAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, StudyTeknik.Auth.DevAuthHandler>(
                    StudyTeknik.Auth.DevAuthHandler.Scheme, _ => { });
        }
        else
        {
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = builder.Configuration["Jwt:Authority"];
                    options.Audience  = builder.Configuration["Jwt:Audience"];
                    options.RequireHttpsMetadata = true;
                });
        }

        
        // AuthN (extern IdP – byt Authority/Audience i appsettings)
        //builder.Services
        //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
       //         options.Authority = builder.Configuration["Jwt:Authority"];
        //        options.Audience = builder.Configuration["Jwt:Audience"];
        //        options.RequireHttpsMetadata = true;
        //    });
        
        // CurrentUserService
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddAuthorization(opts =>
        {
            // Dev-vänlig variant för Users.Manage
            opts.AddPolicy("Users.Manage", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(ctx =>
                    // DevAuth: Admin-roll
                    ctx.User.IsInRole("Admin")
                    // Bearer: scope-claim innehåller users:manage
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
                    // DevAuth: tillåt alla inloggade roller att läsa
                    ctx.User.IsInRole("Admin")  ||
                    ctx.User.IsInRole("Teacher")||
                    ctx.User.IsInRole("Mentor") ||
                    ctx.User.IsInRole("Student")
                    // Bearer/JWT: alternativ via scope-claim
                    || ctx.User.HasClaim(c =>
                        c.Type == "scope" &&
                        c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Contains("users:read"))
                );
            });

            // (dina övriga policies)
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
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateStudentHandler).Assembly));
        
        // FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddValidatorsFromAssembly (typeof(CreateStudentHandler).Assembly);

        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
       builder.Services.AddSwaggerGen(c =>
{
    if (builder.Environment.IsDevelopment())
    {
        // X-UserId
        c.AddSecurityDefinition("X-UserId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "X-UserId",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Description = "Dev auth: seed GUID för användare"
        });

        // X-Role
        c.AddSecurityDefinition("X-Role", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "X-Role",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Description = "Dev auth: Student | Teacher | Mentor | Admin"
        });

        // Kräv båda scheman för alla operationer
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "X-UserId" }}, new string[] {}
            },
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "X-Role" }}, new string[] {}
            }
        });
    }
    else
    {
        // PROD: Bearer/JWT i Swagger
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
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }}, new string[] {}
            }
        });
    }
});
      
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            // Seed some testdata
            using var scope = app.Services.CreateScope();
            var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            await DatabaseSeeder.SeedAsync(db, logger);
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ForbiddenLoggingMiddleware>();
        
        app.MapControllers();
        
          

        app.Run();
    }
}