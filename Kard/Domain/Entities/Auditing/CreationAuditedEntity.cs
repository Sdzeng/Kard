using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Domain.Entities.Auditing
{
    public class CreationAuditedEntity: ICreationAuditedEntity
    {
        public virtual long? Creator { get; set; }
        public virtual DateTime CreationTime { get; set; }

    }
}
