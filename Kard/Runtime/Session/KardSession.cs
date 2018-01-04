using Kard.DI;
using Kard.Runtime.Security;
using System.Linq;

namespace Kard.Runtime.Session
{
    public class KardSession : IKardSession, ISingletonService
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

                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.IsLogin);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                bool isLogin;
                if (!bool.TryParse(userIdClaim.Value, out isLogin))
                {
                    return null;
                }

                return isLogin;
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

        /// <summary>
        /// 登陆名
        /// </summary>
        public virtual string Name
        {
            get
            {

                var userNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.Name);
                if (string.IsNullOrEmpty(userNameClaim?.Value))
                {
                    return null;
                }

                return userNameClaim.Value;
            }
        }


        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string NikeName
        {
            get
            {

                var nameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.NikeName);
                if (string.IsNullOrEmpty(nameClaim?.Value))
                {
                    return null;
                }

                return nameClaim.Value;
            }
        }


        /// <summary>
        /// 手机
        /// </summary>
        public virtual string Phone
        {
            get
            {

                var nameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.Phone);
                if (string.IsNullOrEmpty(nameClaim?.Value))
                {
                    return null;
                }

                return nameClaim.Value;
            }
        }


        /// <summary>
        /// 邮箱
        /// </summary>
        public virtual string Email
        {
            get
            {

                var nameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == KardClaimTypes.Email);
                if (string.IsNullOrEmpty(nameClaim?.Value))
                {
                    return null;
                }

                return nameClaim.Value;
            }
        }

 
    }
}
