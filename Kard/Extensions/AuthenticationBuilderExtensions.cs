using Kard.Runtime.Security.Authentication.WeChat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Extensions
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddWeChatApp(this AuthenticationBuilder builder)
          => builder.AddWeChatApp(WeChatAppDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddWeChatApp(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddWeChatApp(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddWeChatApp(this AuthenticationBuilder builder, Action<WeChatAppOptions> configureOptions)
            => builder.AddWeChatApp(WeChatAppDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChatApp(this AuthenticationBuilder builder, string authenticationScheme, Action<WeChatAppOptions> configureOptions)
            => builder.AddWeChatApp(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddWeChatApp(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeChatAppOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WeChatAppOptions>, WeChatAppPostConfigureOptions>());
            return builder.AddScheme<WeChatAppOptions, CookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
