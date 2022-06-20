using Microsoft.AspNetCore.Http;
using NetCoreCleanArchitecture.Application.Common.Identities;
using System.Security.Claims;

namespace NetCoreCleanArchitecture.Api.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
