using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Runtime.Security.Authentication.WeChat
{
    public class WeChatAppOptions : OAuthOptions
    {
        /// <summary>
        /// Initializes a new <see cref="GoogleOptions"/>.
        /// </summary>
        public WeChatAppOptions()
        {
            CallbackPath = new PathString("/signin-google");
            AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint;
            TokenEndpoint = GoogleDefaults.TokenEndpoint;
            UserInformationEndpoint = GoogleDefaults.UserInformationEndpoint;
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
            ClaimActions.MapJsonSubKey(ClaimTypes.GivenName, "name", "givenName");
            ClaimActions.MapJsonSubKey(ClaimTypes.Surname, "name", "familyName");
            ClaimActions.MapJsonKey("urn:google:profile", "url");
            ClaimActions.MapCustomJson(ClaimTypes.Email, GoogleHelper.GetEmail);
        }

        /// <summary>
        /// access_type. Set to 'offline' to request a refresh token.
        /// </summary>
        public string AccessType { get; set; }
    }
}
