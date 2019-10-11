using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HeatingApi
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string permissionName)
        {
            var policy = new AuthorizationPolicyBuilder().RequireClaim(ClaimTypes.Role, permissionName)
                                                         .Build();

            return Task.FromResult(policy);
        }
    }
}
