using Application.Abstractions;                   // IDateTimeProvider, IAuditLogger
using Application.Abstractions.IPersistence;
using Application.Abstractions.IPersistence.Repositories; // IAppDbContext
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories; // AppDbContext
using Infrastructure.Service;                    // DateTimeProvider, AuditLogger  (ev. Infrastructure.Service)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    // TODO: Kallas från Web.Program.cs → services.AddInfrastructure(Configuration);
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("connection string not found");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IAuditLogger, AuditLogger>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<IMentorRepository, MentorRepository>();


            return services;
        }
    }
}