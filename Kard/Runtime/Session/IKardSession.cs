using Kard.DI;

namespace Kard.Runtime.Session
{
    public interface IKardSession:ISingletonService
    {
        /// <summary>
        /// 是否已登陆
        /// </summary>
        bool? IsLogin { get; }

        /// <summary>
        /// 认证类型
        /// </summary>
        string AuthenticationType { get; }

        /// <summary>
        /// 用户编号
        /// </summary>
        long? UserId { get; }


        /// <summary>
        /// 微信UnionId
        /// </summary>
        string WxUnionId { get; }



        //string WxSessionKey { get; }
        /// <summary>
        /// 登陆名
        /// </summary>
        string Name { get; }


        SessionData Data { get; }

        void RefreshData();
    }
}
