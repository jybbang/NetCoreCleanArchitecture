using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Behaviours;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.Options;
using System.Reflection;

namespace NetCoreCleanArchitecture.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCleanApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<EventBufferService>();

            services.Configure<EventBufferServiceOptions>(configuration.GetSection(nameof(EventBufferServiceOptions)));

            services.AddScoped<IApplicationEventSource, ApplicationEventSource>();

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
