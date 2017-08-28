using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class LastModificationAuditedEntity : CreationAuditedEntity, ILastModificationAuditedEntity, ICreationAuditedEntity
    {
        public virtual long? LastModifier { get; set; }
        public virtual DateTime? LastModificationTime { get; set; }
    }
}
