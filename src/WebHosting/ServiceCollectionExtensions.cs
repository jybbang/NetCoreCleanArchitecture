﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.WebHosting.Filters;
using NetCoreCleanArchitecture.WebHosting.Identity;
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

            // AddCorsAllowAny
            services.AddCors(options => options.AddPolicy("AllowAny",
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            // ConfigureFormOptionsMaxLengthLimit
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
            });

            return services;
        }

        public static IMvcBuilder AddNetCleanControllers(this IServiceCollection services)
        {
            // Controller with custom validator
            var builder = services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddJsonOptions(opt => opt.JsonSerializerOptions.IgnoreNullValues = true)
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
