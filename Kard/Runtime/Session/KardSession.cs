using Kard.DI;
using Kard.Runtime.Security;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace Kard.Runtime.Session
{
    public abstract class KardSession : IKardSession
    {
     
        public KardSession(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor = principalAccessor;
         
        }


        protected IPrincipalAccessor PrincipalAccessor { get; }

        public bool? IsLogin
        {
            get
            {
                return PrincipalAccessor.Principal?.Identity.IsAuthenticated;
                //var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.IsLogin);
                //if (string.IsNullOrEmpty(userIdClaim?.Value))
                //{
                //    return null;
                //}

                //bool isLogin;
                //if (!bool.TryParse(userIdClaim.Value, out isLogin))
                //{
                //    return null;
                //}

                //return isLogin;
            }
        }

        public string AuthenticationType
        {
            get
            {
                return PrincipalAccessor.Principal?.Identity.AuthenticationType;
            }
        }

        public long? UserId
        {
            get
            {

                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.UserId);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public  string WxUnionId
        {
            get
            {

                var wxOpenIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.WxUnionId);
                if (string.IsNullOrEmpty(wxOpenIdClaim?.Value))
                {
                    return null;
                }

                return wxOpenIdClaim.Value;
            }
        }


        /// <summary>
        /// 登陆名
        /// </summary>
        public virtual string Name
        {
            get
            {

                var nameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.Name);
                if (string.IsNullOrEmpty(nameClaim?.Value))
                {
                    return null;
                }

                return nameClaim.Value;
            }
        }

        public abstract SessionData Data { get; }


        public abstract void RefreshData();
        

    }

         
}
