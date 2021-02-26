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

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreCleanArchitectureHosting(this IServiceCollection services, IConfiguration configuration)
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
