namespace Kard.Runtime.Session
{
    public interface IKardSession//: IAbpSession
    {
        /// <summary>
        /// 是否已登陆
        /// </summary>
        bool? IsLogin { get; }
        /// <summary>
        /// 用户编号
        /// </summary>
        long? UserId { get; }

        string WxOpenId { get; }
        /// <summary>
        /// 登陆名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        string NikeName { get; }

        /// <summary>
        /// 手机
        /// </summary>
        string Phone { get; }

        /// <summary>
        /// 邮箱
        /// </summary>
        string Email { get; }

       



      
    }
}
