using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.Identities;
using Results.Fluent;

namespace NetCoreCleanArchitecture.Application.Common.Dummies
{
    public class DummyIdentityService : IIdentityService
    {
        public ValueTask<bool> AuthorizeAsync(string userId, string policyName)
        {
            return new ValueTask<bool>(true);
        }

        public ValueTask<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            return new ValueTask<(Result Result, string UserId)>((Result.Success(), userName));
        }

        public ValueTask<Result> DeleteUserAsync(string userId)
        {
            return new ValueTask<Result>(Result.Success());
        }

        public ValueTask<string> GetUserNameAsync(string userId)
        {
            return new ValueTask<string>(userId);
        }

        public ValueTask<bool> IsInRoleAsync(string userId, string role)
        {
            return new ValueTask<bool>(true);
        }
    }
}
