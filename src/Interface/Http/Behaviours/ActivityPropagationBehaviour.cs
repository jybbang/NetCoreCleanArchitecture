using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreCleanArchitecture.Application.Identities;
using OpenTelemetry.Trace;

namespace NetCoreCleanArchitecture.Interface.Http.Behaviours
{
    public class ActivityPropagationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Tracer _tracer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public ActivityPropagationBehaviour(Tracer tracer, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _tracer = tracer;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_tracer is null)
            {
                return await next();
            }

            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            using var span = _tracer!.StartActiveSpan(requestName);

            try
            {
                span.SetAttribute("userId", userId);

                span.SetAttribute("userName", userName);

                return await next();
            }
            catch (Exception ex)
            {
                span.SetStatus(Status.Error);

                span.RecordException(ex);

                throw;
            }
        }
    }
}
