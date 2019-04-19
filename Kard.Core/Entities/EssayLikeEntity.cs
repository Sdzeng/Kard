using Kard.Domain.Entities.Auditing;

namespace Kard.Core.Entities
{
    public class EssayLikeEntity : CreationAuditedEntity
    {

        public long Id { get; set; }
        public long EssayId { get; set; }

        //public KuserEntity Kuser;
    }

 
}
