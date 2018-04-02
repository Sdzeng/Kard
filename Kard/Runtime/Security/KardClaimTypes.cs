 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Runtime.Security
{
    public static class KardClaimTypes
    {

        /// <summary>
        /// 是否已登陆
        /// </summary>
        public const string IsLogin = "http://www.localyc.com/identity/claims/islogin";

        /// <summary>
        /// 用户编号
        /// </summary>
        public const string UserId = ClaimTypes.NameIdentifier;

        public const string WxOpenId= "http://www.localyc.com/identity/claims/wxopenid";

        /// <summary>
        /// 登陆名
        /// </summary>
        public const string Name = ClaimTypes.Name;

        /// <summary>
        /// 昵称
        /// </summary>
        public const string NikeName = "http://www.localyc.com/identity/claims/nikename";


        /// <summary>
        /// 手机
        /// </summary>
        public const string Phone = "http://www.localyc.com/identity/claims/nikename";


        /// <summary>
        /// 邮箱
        /// </summary>
        public const string Email = ClaimTypes.Email;


   
    }
}
