using Kard.Domain.Entities.Auditing;
using System;
using System.Security.Principal;

namespace Kard.Core.Entities
{
    //Table:user
    public class KuserEntity :  IIdentity
    {

        public KuserEntity()
        {

        }


        public long Id { get; set; }

        public string UserType { get; set; }
        public string WxOpenId { get; set; }
        public string WxSessionKey { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string AvatarUrl { get; set; }

        public string Introduction { get; set; }

        //public string Email { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }

        public string CoverPath { get; set; }
        public int? Gender { get; set; }

        public string City { get; set; }
        public string Language { get; set; }
        public int FollowNum { get; set; }
        public int LikeNum { get; set; }
        public int FansNum { get; set; }

        public int KroleId { get; set; }

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public DateTime CreationTime { get; set; }

        //public KuserEntity ToSecurity() {
        //    this.Password = "******";
        //    this.WxSessionKey = "******";
        //    this.WxOpenId = "******";
        //    return this;
        //}
    }



}