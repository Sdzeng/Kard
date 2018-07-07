using DapperExtensionsCore.Mapper;
using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
    public class EssayCommentEntity: DeletionAuditedEntity
    {
        public long Id { get; set; }

        public long? ParentId { get; set; }

        public long EssayId { get; set; }

        public string Content { get; set; }

        public long LikeNum { get; set; }

        //public KuserEntity Kuser;

        //public IEnumerable<EssayCommentEntity> EssayCommentEntityList { get; set; }
    }

    public class EssayCommentMapper : ClassMapper<EssayCommentEntity>
    {

        public EssayCommentMapper()
        {
            Table("essay_comment");
            //Map(e => e.Kuser).Ignore();
            //Map(e => e.EssayCommentEntityList).Ignore();
            AutoMap();
        }
    }
}
