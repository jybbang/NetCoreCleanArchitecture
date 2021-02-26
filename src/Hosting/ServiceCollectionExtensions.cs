using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Hosting.Filters;
using DaprCleanArchitecture.Hosting.Identity;
using DotNetCore.AspNetCore;
using DotNetCore.Security;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DaprCleanArchitecture.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDaprCleanArchitectureHosting(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Controller with custom validator
            var mvcBuilder = services.AddControllers(options =>
                 options.Filters.Add<ApiExceptionFilterAttribute>())
                    .AddFluentValidation();

            // Dapr to controllers
            mvcBuilder.AddDapr();

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
    }
}
