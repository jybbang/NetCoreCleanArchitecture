using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Rpcs;
using NetCoreCleanArchitecture.Infrastructure.Grpc.EventBus;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanGrpcEventBus(this IServiceCollection services)
        {
            services.AddOptions<GrpcOptions>("Api:Grpc");

            services.AddSingleton<GrpcEventBusRpc>();

            // EventBus
            services.AddScoped<IEventBus, GrpcEventBus>();

            return services;
        }
    }
}
