using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Entities
{
    public class EssayCommentEntity: DeletionAuditedEntity
    {
        public long Id { get; set; }

        public long ParentId { get; set; }

        public long EssayId { get; set; }

        public string Content { get; set; }

        public long LikeNum { get; set; }

    }
}
