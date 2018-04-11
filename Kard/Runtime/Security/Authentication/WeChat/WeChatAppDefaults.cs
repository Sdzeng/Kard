using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Runtime.Security.Authentication.WeChat
{
    public static class WeChatAppDefaults
    {

        public const string AuthenticationScheme = "WeChatApp";

        public static readonly string DisplayName = "WeChatApp";

        public static readonly string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";

        public static readonly string TokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";

        public static readonly string UserInformationEndpoint = "https://www.googleapis.com/plus/v1/people/me";
    }
}
