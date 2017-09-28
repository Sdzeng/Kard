using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kard.Web.Middlewares.ApiAuthorization
{
    public static class ApiAuthorizationExtensions
    {
        public static IApplicationBuilder UseApiAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<ApiAuthorizationMiddleware>();
        }





        public static IApplicationBuilder UseApiAuthorization(this IApplicationBuilder app, ApiAuthorizationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.PathMatch.HasValue && options.PathMatch.Value.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("The path must not end with a '/'", nameof(options.PathMatch));
            }

           
            return app.UseMiddleware<ApiAuthorizationMiddleware>(Options.Create(options));
        }
    }
}
