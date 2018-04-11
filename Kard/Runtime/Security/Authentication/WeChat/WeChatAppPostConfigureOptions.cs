using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Runtime.Security.Authentication.WeChat
{
    public class WeChatAppPostConfigureOptions : IPostConfigureOptions<WeChatAppOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public WeChatAppPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }

        /// <summary>
        /// Invoked to post configure a TOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, WeChatAppOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;

            if (string.IsNullOrEmpty(options.Cookie.Name))
            {
                options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + name;
            }
            if (options.TicketDataFormat == null)
            {
                // Note: the purpose for the data protector must remain fixed for interop to work.
                var dataProtector = options.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", name, "v2");
                options.TicketDataFormat = new TicketDataFormat(dataProtector);
            }
            if (options.CookieManager == null)
            {
                options.CookieManager = new ChunkingCookieManager();
            }
            if (!options.LoginPath.HasValue)
            {
                options.LoginPath = CookieAuthenticationDefaults.LoginPath;
            }
            if (!options.LogoutPath.HasValue)
            {
                options.LogoutPath = CookieAuthenticationDefaults.LogoutPath;
            }
            if (!options.AccessDeniedPath.HasValue)
            {
                options.AccessDeniedPath = CookieAuthenticationDefaults.AccessDeniedPath;
            }
        }

    }
}
