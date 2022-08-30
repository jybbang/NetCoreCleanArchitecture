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
    public class ActivityExceptionPropagationIncomingCallFilter : IIncomingGrainCallFilter
    {
        private readonly ActivitySource activitySource;

        public ActivityExceptionPropagationIncomingCallFilter(ActivitySource activitySource)
        {
            this.activitySource = activitySource;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                await context.Invoke();

                using var activity = Activity.Current;

                if (activity?.IsAllDataRequested == true
                    && activity.GetStatus() == Status.Unset)
                {
                    activity.SetTag("rpc.system", "orleans");
                    activity.SetTag("rpc.service", context.InterfaceMethod?.DeclaringType?.FullName);
                    activity.SetTag("rpc.method", context.InterfaceMethod?.Name);
                    activity.SetStatus(Status.Ok);
                }
            }
            catch (Exception ex)
            {
                using var activity = Activity.Current;

                if (activity?.IsAllDataRequested == true
                    && activity.GetStatus() != Status.Error)
                {
                    activity.SetTag("rpc.system", "orleans");
                    activity.SetTag("rpc.service", context.InterfaceMethod?.DeclaringType?.FullName);
                    activity.SetTag("rpc.method", context.InterfaceMethod?.Name);
                    activity.SetStatus(Status.Error);
                    activity.RecordException(ex);
                }

                throw;
            }
        }
    }
}
