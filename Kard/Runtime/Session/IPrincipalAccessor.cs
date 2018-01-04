using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Runtime.Session
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
