using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.Identities;
using OpenTelemetry.Trace;
using Orleans;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.Common.CallFilters
{
    public class ActivityPropagationGrainCallFilter : IIncomingGrainCallFilter
    {
        private readonly ActivitySource _tracer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public ActivityPropagationGrainCallFilter(ActivitySource tracer, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _tracer = tracer;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            if (_tracer is null)
            {
                await context.Invoke();
            }

            var requestName = context.ImplementationMethod.Name;
            var grainName = context.Grain.GetType().Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            using var span = _tracer!.StartActivity(requestName);

            try
            {
                span?.SetTag("userId", userId);

                span?.SetTag("userName", userName);

                span?.SetTag("grain", grainName);

                await context.Invoke();

                span?.SetStatus(Status.Ok);
            }
            catch (Exception ex)
            {
                span?.SetStatus(Status.Error);

                span?.RecordException(ex);

                throw;
            }
        }
    }
}
