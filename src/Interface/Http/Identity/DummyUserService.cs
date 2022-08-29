using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NetCoreCleanArchitecture.Application.Identities;

namespace NetCoreCleanArchitecture.Interface.Http.Identity
{
    public class DummyUserService : ICurrentUserService
    {
        public string? UserId => string.Empty;
    }
}
