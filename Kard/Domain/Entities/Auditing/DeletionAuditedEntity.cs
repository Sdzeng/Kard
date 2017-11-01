using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class DeletionAuditedEntity: CreationAuditedEntity, IDeletionAuditedEntity, ICreationAuditedEntity
    {
        public virtual long? DeleterUserId { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
