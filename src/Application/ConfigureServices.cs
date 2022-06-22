using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Behaviours;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using System.Reflection;

namespace NetCoreCleanArchitecture.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanApplication(this IServiceCollection services)
        {
            services.AddSingleton<EventBufferService>();

            services.AddScoped<IApplicationEventSource, ApplicationEventSource>();

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            return services;
        }
    }
}
