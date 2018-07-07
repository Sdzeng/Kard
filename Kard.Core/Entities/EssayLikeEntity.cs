using DapperExtensionsCore.Mapper;
using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
    public class EssayLikeEntity : CreationAuditedEntity
    {

        public long Id { get; set; }
        public long EssayId { get; set; }

        public KuserEntity Kuser;
    }

    //public class EssayLikeMapper : ClassMapper<EssayLikeEntity>
    //{

    //    public EssayLikeMapper()
    //    {
    //        Table("essay_like");
    //        Map(e => e.Kuser).Ignore();
    //        AutoMap();
    //    }
    //}
}
