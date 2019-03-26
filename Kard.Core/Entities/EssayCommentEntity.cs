using Kard.Domain.Entities.Auditing;

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


}
