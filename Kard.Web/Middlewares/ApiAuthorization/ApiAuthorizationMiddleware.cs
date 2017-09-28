using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Kard.Web.Middlewares.ApiAuthorization
{
    public class ApiAuthorizationMiddleware
    {
        private readonly ApiAuthorizationOptions _options;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the ApiAuthorizedMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="hostingEnv">The <see cref="IHostingEnvironment"/> used by this middleware.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory"/> instance used to create loggers.</param>
        public ApiAuthorizationMiddleware(RequestDelegate next, IHostingEnvironment hostingEnv, IOptions<ApiAuthorizationOptions> options, ILoggerFactory loggerFactory)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (hostingEnv == null)
            {
                throw new ArgumentNullException(nameof(hostingEnv));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _next = next;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<ApiAuthorizationMiddleware>();
        }

        /// <summary>
        /// Processes a request to determine if it matches a known file, and if so, serves it.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Path.StartsWithSegments(_options.PathMatch))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("You are not authorized!");
                return;
            }

            await _next(context);
        }
    }
}
