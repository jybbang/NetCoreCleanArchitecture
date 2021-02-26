using DaprCleanArchitecture.Application.Common.Behaviours;
using DaprCleanArchitecture.Application.Common.EventSources;
using DaprCleanArchitecture.Application.Common.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DaprCleanArchitecture.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDaprCleanArchitectureApplication(this IServiceCollection services)
        {
            services.AddScoped<IApplicationEventSource, ApplicationEventSource>();

            services.AddScoped<IApplicationContext, ApplicationContext>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            return services;
        }
    }
}
