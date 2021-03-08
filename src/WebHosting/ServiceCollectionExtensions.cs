using DotNetCore.AspNetCore;
using DotNetCore.Security;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.WebHosting.Filters;
using NetCoreCleanArchitecture.WebHosting.Identity;
using System;
using Prometheus;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCleanWebHosting(this IServiceCollection services)
        {
            // Identity
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // DotNetCore.AspNetCore
            services.AddAuthenticationJwtBearer();

            services.AddCorsAllowAny();

            services.AddFileExtensionContentTypeProvider();

            services.ConfigureFormOptionsMaxLengthLimit();

            // DotNetCore.Security
            services.AddHash();

            services.AddJsonWebToken(Guid.NewGuid().ToString(), TimeSpan.FromHours(12));

            services.AddAuthenticationJwtBearer();

            return services;
        }

        public static IMvcBuilder AddNetCleanControllers(this IServiceCollection services)
        {
            // Controller with custom validator
            var builder = services.AddControllers(options =>
                     options.Filters.Add<ApiExceptionFilterAttribute>())
                        .AddFluentValidation();

            return builder;
        }

        public static IHealthChecksBuilder AddNetCleanHealthChecks(this IServiceCollection services)
        {
            // ASP.NET Core health check status metrics
            var builder = services.AddHealthChecks()
                .ForwardToPrometheus();

            return builder;
        }
    }
}
