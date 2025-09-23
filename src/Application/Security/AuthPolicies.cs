using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Linq;


namespace Application.Security
{
    public static class AuthPolicies
    {
        public static AuthorizationPolicy RequireScope(string scope) =>
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == "scope" &&
                                           c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Contains(scope)))
                .Build();
    }
}