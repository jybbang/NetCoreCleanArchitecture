using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.Identities;
using OpenTelemetry.Trace;
using Orleans;
using Orleans.Runtime;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.Common.CallFilters
{
    public class ActivityPropagationIncomingGrainCallFilter : IIncomingGrainCallFilter
    {
        private readonly ActivitySource activitySource;

        public ActivityPropagationIncomingGrainCallFilter(ActivitySource activitySource)
        {
            this.activitySource = activitySource;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            var method = context.InterfaceMethod?.Name ?? "Orleans.Runtime.GrainCall";

            var activity = activitySource.CreateActivity(method, ActivityKind.Server);

            if (activity is null)
            {
                await context.Invoke();
                return;
            }

            activity.Start();

            try
            {
                // rpc attributes from https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/rpc.md
                activity.SetTag("rpc.system", "orleans");
                activity.SetTag("rpc.service", context.InterfaceMethod?.DeclaringType?.FullName);
                activity.SetTag("rpc.method", method);
                activity.SetTag("net.peer.name", context.Grain?.ToString());

                await context.Invoke();
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
