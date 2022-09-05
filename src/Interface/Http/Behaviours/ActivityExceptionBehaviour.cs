using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenTelemetry.Trace;

namespace NetCoreCleanArchitecture.Interface.Http.Behaviours
{
    public class ActivityExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var result = await next();

                using var activity = Activity.Current;

                if (activity?.IsAllDataRequested == true
                    && activity.GetStatus() == Status.Unset)
                {
                    activity.SetStatus(Status.Ok);
                }

                return result;
            }
            catch (Exception ex)
            {
                using var activity = Activity.Current;

                if (activity?.IsAllDataRequested == true
                    && activity.GetStatus() != Status.Error)
                {
                    activity.SetTag("exception.peer.name", typeof(TRequest).Name);
                    activity.SetStatus(Status.Error);
                    activity.RecordException(ex);
                }

                throw;
            }
        }
    }
}
