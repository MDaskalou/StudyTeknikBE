using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories;
using Infrastructure.DependencyInjection;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StudyTeknik.Middleware;
using StudyTeknik.Service;

namespace StudyTeknik;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        

        // Add services to the container.
        builder.Services.AddControllers();
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
            opts.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
            opts.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
            opts.AddPolicy("MentorOnly", policy => policy.RequireRole("Mentor"));
            opts.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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