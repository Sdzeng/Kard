using Kard.Domain.Entities.Auditing;
using System.Security.Principal;

namespace Kard.Core.Entities
{
    //Table:user
    public class KuserEntity : LastModificationAuditedEntity, ILastModificationAuditedEntity, IIdentity
    {

        public KuserEntity()
        {

        }


        public int Id { get; set; }

        public string WxOpenId { get; set; }


        public string Name { get; set; }

        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NikeName { get; set; }

        public string CoverPath { get; set; }
        public string Gender { get; set; }

        public string City { get; set; }
        public string Language { get; set; }


        public int ExperienceValue { get; set; }
        public int KroleId { get; set; }

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

    }


    //public class KuserMapper : ClassMapper<KuserEntity>
    //{

    //    public KuserMapper()
    //    {
    //        Table("kuser");
    //        Map(e => e.AuthenticationType).Ignore();
    //        Map(e => e.IsAuthenticated).Ignore();
    //        AutoMap();
    //    }
    //}

}