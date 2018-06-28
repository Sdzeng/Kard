using System;

namespace Kard.Domain.Entities.Auditing
{
    public class DeletionAuditedEntity: CreationAuditedEntity, IDeletionAuditedEntity, ICreationAuditedEntity
    {
        public virtual long? DeleterUserId { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }

        public void AuditDeletion(long userId)
        {
            this.DeleterUserId = userId;
            this.DeletionTime = DateTime.Now;
            this.IsDeleted = true;
            
        }
    }
}
