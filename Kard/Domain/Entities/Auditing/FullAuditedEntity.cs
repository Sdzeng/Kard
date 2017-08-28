using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class FullAuditedEntity : LastModificationAuditedEntity, IFullAuditedEntity
    {
        public virtual long? Deleter { get; set; }
        public virtual DateTime? DeletionTime { get; set; }
        public virtual bool IsDeleted { get; set; }
    }

}
