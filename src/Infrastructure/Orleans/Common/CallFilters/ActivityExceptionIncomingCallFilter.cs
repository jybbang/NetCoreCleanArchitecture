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
    public class ActivityExceptionIncomingCallFilter : IIncomingGrainCallFilter
    {
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
                    activity.SetTag("exception.service", context.InterfaceMethod?.DeclaringType?.FullName);
                    activity.SetTag("exception.method", context.InterfaceMethod?.Name);
                    activity.SetTag("exception.peer.name", context.Grain?.ToString());
                    activity.SetStatus(Status.Error);
                    activity.RecordException(ex);
                }

                throw;
            }
        }
    }
}
