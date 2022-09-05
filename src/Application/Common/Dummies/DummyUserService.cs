using NetCoreCleanArchitecture.Application.Identities;

namespace NetCoreCleanArchitecture.Application.Common.Dummies
{
    public class DummyUserService : ICurrentUserService
    {
        public string? UserId => string.Empty;
    }
}
