using DaprCleanArchitecture.Application;
using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Endpoint.Filters;
using DaprCleanArchitecture.Endpoint.Identity;
using DaprCleanArchitecture.Infrastructure;
using DotNetCore.AspNetCore;
using DotNetCore.Security;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DaprCleanArchitecture.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Application Layer
            services.AddApplicationLayer();

            // Infrastructure Layer
            services.AddInfrastructureLayer(Configuration);

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

            // Swagger
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = $"{nameof(DaprCleanArchitecture)} - {Program.AppName} API";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            // DotNetCore.AspNetCore
            app.UseCorsAllowAny();

            // CloudEvents
            app.UseCloudEvents();

            // Endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();
            });

            // Swagger
            app.UseStaticFiles();

            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/api";
                settings.DocumentPath = "/api/specification.json";
            });
        }
    }
}
