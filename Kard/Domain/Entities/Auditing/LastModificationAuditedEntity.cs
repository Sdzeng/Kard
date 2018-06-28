using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class LastModificationAuditedEntity : CreationAuditedEntity, ILastModificationAuditedEntity, ICreationAuditedEntity
    {
        public virtual long? LastModifierUserId { get; set; }
        public virtual DateTime? LastModificationTime { get; set; }

        public void AuditLastModification(long userId)
        {
            this.LastModifierUserId = userId;
            this.LastModificationTime = DateTime.Now;
       
        }
    }
}
