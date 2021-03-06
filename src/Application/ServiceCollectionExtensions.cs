﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Behaviours;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using System.Reflection;

namespace NetCoreCleanArchitecture.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCleanApplication(this IServiceCollection services)
        {
            services.AddScoped<IApplicationEventSource, ApplicationEventSource>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            return services;
        }
    }
}
