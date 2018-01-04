using Kard.DI;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;

namespace Kard.Runtime.Session
{

    public class AspNetCorePrincipalAccessor : IPrincipalAccessor, ISingletonService
    {

        public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? null;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

    }
}
