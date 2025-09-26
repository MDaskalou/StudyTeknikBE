using Application.Abstractions;                    // IDateTimeProvider, IAuditLogger
using Application.Abstractions.IPersistence;       // IAppDbContext
using Application.Abstractions.IPersistence.Repositories;
using Application.Student.Repository; // repo-interfacen (Application-lagret)
using Infrastructure.Persistence;                  // AppDbContext
using Infrastructure.Persistence.Repositories;     // repo-implementationer (Infrastructure)
using Infrastructure.Service;                      // DateTimeProvider, AuditLogger
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // Registrerar allt som tillhör Infrastruktur-lagret:
        // - DbContext (EF Core + SQL Server)
        // - IAppDbContext (abstraktion till samma DbContext)
        // - Tekniska tjänster (tid, audit)
        // - Repository-implementationer (EF mot DbContext)
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) Anslutning till databasen (SQL Server)
            var cs = configuration.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(cs, sql =>
                {
                    // Sätter migrations-assembly = Infrastructure (där AppDbContext finns)
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

            // 2) Binda abstraktionen IAppDbContext till samma instans av AppDbContext (Scoped per request)
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            // 3) Tekniska infrastruktur-tjänster (tidskälla, audit-loggning)
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IAuditLogger, AuditLogger>();

            // 4) Repository-implementationer (Infrastructure) kopplas till Application-interfacen
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<IMentorRepository, MentorRepository>();

            // Lägg till STUDENT-repository (för Get All Students)
            services.AddScoped<IStudentRepository, StudentRepository>();

            // (Ev. fler repos här …)

            return services;
        }
    }
}
