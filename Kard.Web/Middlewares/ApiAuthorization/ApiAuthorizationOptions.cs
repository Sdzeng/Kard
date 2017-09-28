using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kard.Web.Middlewares.ApiAuthorization
{
    public class ApiAuthorizationOptions
    {
        public ApiAuthorizationOptions()
        {
            //OnPrepareResponse = _ => { };
        }

        public PathString PathMatch { get; set; }

        //public Action<ApiAuthorizedResponseContext> OnPrepareResponse { get; set; }
    }
}
