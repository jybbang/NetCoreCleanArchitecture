﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NetCoreCleanArchitecture.Application.Identities;

namespace NetCoreCleanArchitecture.Interface.Http.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
