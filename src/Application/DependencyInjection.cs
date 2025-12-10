using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Common.Behaviors;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 1. Konfigurera MediatR
            services.AddMediatR(cfg => 
            {
                // Registrera alla Handlers automatiskt
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                // Registrera din Pipeline Behavior (Validering)
                // Detta gör att validering körs automatiskt innan varje Handler
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            
            // 2. Konfigurera FluentValidation
            // Registrera alla Validators automatiskt
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}