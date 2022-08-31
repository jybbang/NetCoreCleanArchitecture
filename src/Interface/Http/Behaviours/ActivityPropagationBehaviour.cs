using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using NetCoreCleanArchitecture.Application.Identities;
using OpenTelemetry.Trace;

namespace NetCoreCleanArchitecture.Interface.Http.Behaviours
{
    public class ActivityPropagationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ActivitySource _activitySource;

        public ActivityPropagationBehaviour(ActivitySource activitySource)
        {
            _activitySource = activitySource;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var activity = _activitySource.CreateActivity(typeof(TRequest).Name, ActivityKind.Internal);

            if (activity is null) return await next();

            activity.Start();

            try
            {
                var result = await next();

                if (activity.IsAllDataRequested)
                {
                    activity.SetStatus(Status.Ok);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (activity.IsAllDataRequested)
                {
                    activity.SetStatus(Status.Error);
                    activity.RecordException(ex);
                }

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
